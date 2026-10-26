using System;
using System.Text.Json;
using CreatorNetwork.Contracts;
using BenchmarkDotNet.Attributes;

namespace CreatorNetwork.Benchmarks;

[MemoryDiagnoser]
public class PublicationJsonBenchmarks
{
    private readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web);
    private ChannelPublication _small = null!;
    private ChannelPublication _hundredItems = null!;
    private byte[] _smallJson = Array.Empty<byte>();
    private byte[] _hundredItemsJson = Array.Empty<byte>();

    [GlobalSetup]
    public void Setup()
    {
        var creator = new CreatorIdentity("creator:json-benchmark", "JSON benchmark");
        _small = new ChannelPublication(
            ChannelPublication.CurrentSchema, "publication:small", creator,
            "Small manifest", null, "public", 1, DateTimeOffset.UnixEpoch);
        _smallJson = JsonSerializer.SerializeToUtf8Bytes(_small, _options);

        var content = new ContentReference[100];
        for (int i = 0; i < content.Length; i++)
            content[i] = new ContentReference("content:" + i, "video/mp4", 1024);
        _hundredItems = new ChannelPublication(
            ChannelPublication.CurrentSchema, "publication:100", creator,
            "100-item manifest", null, "public", 1, DateTimeOffset.UnixEpoch, content);
        _hundredItemsJson = JsonSerializer.SerializeToUtf8Bytes(_hundredItems, _options);
    }

    [Benchmark(Baseline = true)]
    public byte[] SerializeSmallManifest() => JsonSerializer.SerializeToUtf8Bytes(_small, _options);

    [Benchmark]
    public byte[] DeserializeSmallManifest() =>
        JsonSerializer.Deserialize<ChannelPublication>(_smallJson, _options)!;

    [Benchmark]
    public byte[] SerializeHundredItemManifest() => JsonSerializer.SerializeToUtf8Bytes(_hundredItems, _options);

    [Benchmark]
    public object DeserializeHundredItemManifest() =>
        JsonSerializer.Deserialize<ChannelPublication>(_hundredItemsJson, _options)!;
}
