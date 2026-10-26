using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CreatorNetwork.Contracts;
using CreatorNetwork.Node;
using BenchmarkDotNet.Attributes;

namespace CreatorNetwork.Benchmarks;

[MemoryDiagnoser]
public class FileStoreReadBenchmarks
{
    private string _directory = null!;
    private FilePublicationStore _store = null!;
    private string _publicationId = "publication:cached-read";

    [GlobalSetup]
    public async Task Setup()
    {
        _directory = Path.Combine(Path.GetTempPath(), "creator-network-benchmarks", Guid.NewGuid().ToString("N"));
        _store = new FilePublicationStore(_directory, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema, _publicationId,
            new CreatorIdentity("creator:benchmark"), "Cached read", null,
            "public", 1, DateTimeOffset.UnixEpoch);
        await _store.PutAsync(publication, CancellationToken.None);
        await _store.GetAsync(_publicationId, CancellationToken.None);
    }

    [Benchmark]
    public Task<ChannelPublication?> ReadCachedPublication() =>
        _store.GetAsync(_publicationId, CancellationToken.None);

    [GlobalCleanup]
    public void Cleanup()
    {
        if (Directory.Exists(_directory))
            Directory.Delete(_directory, recursive: true);
    }
}
