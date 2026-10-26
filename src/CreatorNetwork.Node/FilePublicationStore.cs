using System.Collections.Concurrent;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CreatorNetwork.Contracts;

namespace CreatorNetwork.Node;

/// <summary>
/// Local-demo store. It caches reads in memory and atomically replaces one manifest file per publication.
/// This implementation is intentionally bound to loopback and is not a production multi-user host.
/// </summary>
public sealed class FilePublicationStore
{
    private readonly string _directory;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ConcurrentDictionary<string, ChannelPublication> _cache = new(StringComparer.Ordinal);
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _writeLocks = new(StringComparer.Ordinal);

    public FilePublicationStore(string directory, JsonSerializerOptions jsonOptions)
    {
        _directory = Path.GetFullPath(directory);
        _jsonOptions = jsonOptions;
        Directory.CreateDirectory(_directory);
    }

    public async Task<ChannelPublication?> GetAsync(string publicationId, CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(publicationId, out ChannelPublication? cached))
            return cached;

        string path = GetPath(publicationId);
        if (!File.Exists(path))
            return null;

        await using var stream = new FileStream(
            path, FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 16 * 1024, FileOptions.Asynchronous | FileOptions.SequentialScan);
        ChannelPublication? publication = await JsonSerializer.DeserializeAsync<ChannelPublication>(
            stream, _jsonOptions, cancellationToken);

        if (publication is null ||
            !string.Equals(publication.PublicationId, publicationId, StringComparison.Ordinal) ||
            PublicationValidator.Validate(publication).Count != 0)
            throw new InvalidDataException("Stored publication is invalid or does not match its requested identifier.");

        return _cache.GetOrAdd(publicationId, publication);
    }

    public async Task PutAsync(ChannelPublication publication, CancellationToken cancellationToken)
    {
        string id = publication.PublicationId;
        SemaphoreSlim gate = _writeLocks.GetOrAdd(id, static _ => new SemaphoreSlim(1, 1));
        await gate.WaitAsync(cancellationToken);
        try
        {
            byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(publication, _jsonOptions);
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
                _cache[id] = publication;
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
