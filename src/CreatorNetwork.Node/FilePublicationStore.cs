using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreatorNetwork.Contracts;

namespace CreatorNetwork.Node;

/// <summary>
/// Local-demo store. It caches validated models, serialized bytes, and ETags, and atomically replaces manifest files.
/// This implementation is intentionally bound to loopback and is not a production multi-user host.
/// </summary>
public sealed class FilePublicationStore
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConcurrentDictionary<string, StoredPublication> _cache = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _writeLocks = new(StringComparer.Ordinal);

    public FilePublicationStore(string directory, JsonSerializerOptions jsonOptions)
    {
        _directory = Path.GetFullPath(directory);
        _jsonOptions = jsonOptions;
        Directory.CreateDirectory(_directory);
    }

    public async Task<ChannelPublication?> GetAsync(string publicationId, CancellationToken cancellationToken)
    {
        StoredPublication? document = await GetDocumentAsync(publicationId, cancellationToken);
        return document?.Publication;
    }

    internal async Task<StoredPublication?> GetDocumentAsync(string publicationId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(publicationId, out StoredPublication? cached))
            return cached;

        string path = GetPath(publicationId);
        if (!File.Exists(path))
            return null;

        byte[] bytes = await File.ReadAllBytesAsync(path, cancellationToken);
        ChannelPublication? publication = JsonSerializer.Deserialize<ChannelPublication>(bytes, _jsonOptions);

        if (publication is null ||
            !string.Equals(publication.PublicationId, publicationId, StringComparison.Ordinal) ||
            PublicationValidator.Validate(publication).Count != 0)
            throw new InvalidDataException("Stored publication is invalid or does not match its requested identifier.");

        var loaded = new StoredPublication(publication, bytes);
        return _cache.GetOrAdd(publicationId, loaded);
    }

    public Task PutAsync(ChannelPublication publication, CancellationToken cancellationToken) =>
        PutAsync(publication, expectedETag: null, createOnly: false, cancellationToken);

    public async Task PutAsync(
        ChannelPublication publication,
        string? expectedETag,
        bool createOnly,
        CancellationToken cancellationToken)
    {
        string id = publication.PublicationId;
        SemaphoreSlim gate = _writeLocks.GetOrAdd(id, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);
        try
        {
            // Revision is provider-enforced ordering, not merely creator-supplied metadata.
            // This in-process gate is sufficient for the single-process loopback prototype;
            // shared-directory multi-process hosting remains unsupported.
            StoredPublication? current = await GetDocumentAsync(id, cancellationToken);
            if (createOnly && current is not null)
                throw new PublicationPreconditionFailedException(id, "The publication already exists.");

            if (!createOnly && expectedETag is not null &&
                (current is null || !string.Equals(expectedETag, current.ETag, StringComparison.Ordinal)))
                throw new PublicationPreconditionFailedException(id, "The supplied If-Match value does not match the current publication.");

            if (current is not null && publication.Revision <= current.Publication.Revision)
                throw new PublicationRevisionConflictException(id, current.Publication.Revision, publication.Revision);

            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(publication, _jsonOptions);
            var document = new StoredPublication(publication, bytes);
            string targetPath = GetPath(id);
            string temporaryPath = targetPath + "." + Guid.NewGuid().ToString("N") + ".tmp";

            try
            {
                await using (var stream = new FileStream(
                    temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None,
                    bufferSize: 16 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan))
                {
                    await stream.WriteAsync(bytes, cancellationToken);
                    await stream.FlushAsync(cancellationToken);
                }

                File.Move(temporaryPath, targetPath, overwrite: true);
                _cache[id] = document;
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

    private string GetPath(string publicationId)
    {
        byte[] idBytes = Encoding.UTF8.GetBytes(publicationId);
        string fileName = Convert.ToHexString(SHA256.HashData(idBytes)).ToLowerInvariant() + ".json";
        return Path.Combine(_directory, fileName);
    }
}

internal sealed class StoredPublication
{
    public StoredPublication(ChannelPublication publication, byte[] jsonBytes)
    {
        Publication = publication;
        JsonBytes = jsonBytes;
        ETag = "\"" + Convert.ToHexString(SHA256.HashData(jsonBytes)).ToLowerInvariant() + "\"";
    }

    public ChannelPublication Publication { get; }
    public byte[] JsonBytes { get; }
    public string ETag { get; }
}

/// <summary>Raised when a publication update does not advance its currently stored revision.</summary>
public sealed class PublicationRevisionConflictException : Exception
{
    public PublicationRevisionConflictException(string publicationId, long currentRevision, long attemptedRevision)
        : base($"Publication '{publicationId}' is at revision {currentRevision}; attempted revision {attemptedRevision} must be greater.")
    {
        PublicationId = publicationId;
        CurrentRevision = currentRevision;
        AttemptedRevision = attemptedRevision;
    }

    public string PublicationId { get; }
    public long CurrentRevision { get; }
    public long AttemptedRevision { get; }
}

/// <summary>Raised when an HTTP conditional publication precondition is not satisfied.</summary>
public sealed class PublicationPreconditionFailedException : Exception
{
    public PublicationPreconditionFailedException(string publicationId, string message)
        : base(message) => PublicationId = publicationId;

    public string PublicationId { get; }
}
