using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreatorNetwork.Contracts;

namespace CreatorNetwork.Node;

/// <summary>
/// Loopback-development media store. Uploads stream through a fixed-size buffer, are hashed
/// while written, and become visible only after the temporary file is finalized.
/// This store has no authentication, quotas per identity, or production authorization.
/// </summary>
public sealed class FileMediaStore
{
    public const long DefaultMaximumAssetBytes = 512L * 1024 * 1024;
    private const int BufferSize = 64 * 1024;

    private readonly string _directory;
    private readonly long _maximumAssetBytes;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _assetLocks = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, MediaAssetDescriptor> _cache = new(StringComparer.Ordinal);
    private readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public FileMediaStore(string directory, long maximumAssetBytes = DefaultMaximumAssetBytes)
    {
        if (maximumAssetBytes <= 0)
            throw new ArgumentOutOfRangeException(nameof(maximumAssetBytes));

        _directory = Path.GetFullPath(directory);
        _maximumAssetBytes = maximumAssetBytes;
        Directory.CreateDirectory(_directory);
    }

    public async Task<MediaAssetDescriptor> PutAsync(
        string assetId,
        string mediaType,
        Stream source,
        long? expectedLength,
        string? expectedSha256,
        CancellationToken cancellationToken)
    {
        ValidateAssetId(assetId);
        if (string.IsNullOrWhiteSpace(mediaType) || mediaType.Length > 255 ||
            mediaType.Any(char.IsControl) || !mediaType.Contains('/'))
            throw new ArgumentException("A valid media type is required.", nameof(mediaType));
        ArgumentNullException.ThrowIfNull(source);
        if (!source.CanRead)
            throw new ArgumentException("The source stream must be readable.", nameof(source));
        if (expectedLength is < 0 || expectedLength > _maximumAssetBytes)
            throw new MediaAssetTooLargeException(_maximumAssetBytes);
        if (expectedSha256 is not null && !IsSha256(expectedSha256))
            throw new ArgumentException("Expected SHA-256 must be 64 hexadecimal characters.", nameof(expectedSha256));

        SemaphoreSlim gate = _assetLocks.GetOrAdd(assetId, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            string contentPath = GetContentPath(assetId);
            string descriptorPath = GetDescriptorPath(assetId);
            if (File.Exists(contentPath) || File.Exists(descriptorPath))
                throw new MediaAssetAlreadyExistsException(assetId);

            string temporaryPath = contentPath + "." + Guid.NewGuid().ToString("N") + ".upload";
            long length = 0;
            string digest;
            try
            {
                using IncrementalHash hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
                byte[] buffer = new byte[BufferSize];
                await using (var destination = new FileStream(
                    temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    while (true)
                    {
                        int read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
                        if (read == 0)
                            break;

                        length += read;
                        if (length > _maximumAssetBytes)
                            throw new MediaAssetTooLargeException(_maximumAssetBytes);
                        if (expectedLength.HasValue && length > expectedLength.Value)
                            throw new MediaAssetIntegrityException("The uploaded byte count exceeds the declared length.");

                        hash.AppendData(buffer, 0, read);
                        await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
                    }

                    await destination.FlushAsync(cancellationToken).ConfigureAwait(false);
                }

                if (expectedLength.HasValue && length != expectedLength.Value)
                    throw new MediaAssetIntegrityException("The uploaded byte count does not match the declared length.");

                digest = Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
                if (expectedSha256 is not null && !CryptographicOperations.FixedTimeEquals(
                        Encoding.ASCII.GetBytes(digest),
                        Encoding.ASCII.GetBytes(expectedSha256.ToLowerInvariant())))
                    throw new MediaAssetIntegrityException("The uploaded bytes do not match the declared SHA-256 digest.");

                var descriptor = new MediaAssetDescriptor(assetId, mediaType.Trim(), length, digest);
                string descriptorTemp = descriptorPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
                try
                {
                    await File.WriteAllTextAsync(
                        descriptorTemp, JsonSerializer.Serialize(descriptor, _jsonOptions), cancellationToken).ConfigureAwait(false);
                    File.Move(temporaryPath, contentPath);
                    try
                    {
                        File.Move(descriptorTemp, descriptorPath);
                    }
                    catch
                    {
                        File.Delete(contentPath);
                        throw;
                    }
                    _cache[assetId] = descriptor;
                    return descriptor;
                }
                finally
                {
                    if (File.Exists(descriptorTemp))
                        File.Delete(descriptorTemp);
                }
            }
            finally
            {
                if (File.Exists(temporaryPath))
                    File.Delete(temporaryPath);
            }
        }
        finally
        {
            gate.Release();
        }
    }

    public async Task<MediaAssetDescriptor?> GetDescriptorAsync(string assetId, CancellationToken cancellationToken)
    {
        ValidateAssetId(assetId);
        if (_cache.TryGetValue(assetId, out MediaAssetDescriptor? cached))
            return cached;

        string contentPath = GetContentPath(assetId);
        string descriptorPath = GetDescriptorPath(assetId);
        if (!File.Exists(contentPath) || !File.Exists(descriptorPath))
            return null;

        await using var descriptorStream = new FileStream(
            descriptorPath, FileMode.Open, FileAccess.Read, FileShare.Read,
            4096, FileOptions.Asynchronous | FileOptions.SequentialScan);
        MediaAssetDescriptor? descriptor = await JsonSerializer.DeserializeAsync<MediaAssetDescriptor>(
            descriptorStream, _jsonOptions, cancellationToken).ConfigureAwait(false);
        if (descriptor is null || !string.Equals(descriptor.AssetId, assetId, StringComparison.Ordinal))
            throw new InvalidDataException("Stored media asset descriptor is invalid.");

        return _cache.GetOrAdd(assetId, descriptor);
    }

    public FileStream? OpenRead(string assetId)
    {
        ValidateAssetId(assetId);
        if (!File.Exists(GetDescriptorPath(assetId)))
            return null;

        string path = GetContentPath(assetId);
        if (!File.Exists(path))
            return null;

        return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read,
            BufferSize, FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    private string GetContentPath(string assetId) => Path.Combine(_directory, GetFileStem(assetId) + ".asset");
    private string GetDescriptorPath(string assetId) => Path.Combine(_directory, GetFileStem(assetId) + ".json");

    private static string GetFileStem(string assetId) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(assetId))).ToLowerInvariant();

    private static void ValidateAssetId(string assetId)
    {
        if (string.IsNullOrWhiteSpace(assetId) || assetId.Length > 256 || assetId.Any(char.IsControl))
            throw new ArgumentException("Asset ID must be non-empty, at most 256 characters, and contain no control characters.", nameof(assetId));
    }

    private static bool IsSha256(string value) =>
        value.Length == 64 && value.All(Uri.IsHexDigit);
}

public sealed class MediaAssetAlreadyExistsException : Exception
{
    public MediaAssetAlreadyExistsException(string assetId)
        : base($"Media asset '{assetId}' already exists.") { }
}

public sealed class MediaAssetTooLargeException : Exception
{
    public MediaAssetTooLargeException(long maximumBytes)
        : base($"Media asset exceeds the configured maximum of {maximumBytes} bytes.") { }
}

public sealed class MediaAssetIntegrityException : Exception
{
    public MediaAssetIntegrityException(string message) : base(message) { }
}
