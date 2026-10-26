using System;
using CreatorNetwork.Contracts;
using BenchmarkDotNet.Attributes;

namespace CreatorNetwork.Benchmarks;

[MemoryDiagnoser]
public class PublicationValidationBenchmarks
{
    private ChannelPublication _valid = null!;
    private ChannelPublication _invalid = null!;
    private ChannelPublication _hundredItems = null!;

    [GlobalSetup]
    public void Setup()
    {
        var creator = new CreatorIdentity("creator:benchmark");
        _valid = new ChannelPublication(
            ChannelPublication.CurrentSchema, "publication:benchmark", creator,
            "Benchmark channel", null, "public", 1, DateTimeOffset.UtcNow);

        _invalid = new ChannelPublication(
            "creator-network.channel/unknown", "", new CreatorIdentity(""),
            "", null, "unknown", 0, DateTimeOffset.UtcNow);

        var items = new ContentReference[100];
        for (int i = 0; i < items.Length; i++)
            items[i] = new ContentReference("content:" + i, "video/mp4", 1024);
        _hundredItems = new ChannelPublication(
            ChannelPublication.CurrentSchema, "publication:100", creator,
            "100-item channel", null, "public", 1, DateTimeOffset.UtcNow, items);
    }

    [Benchmark(Baseline = true)]
    public int ValidateValidManifest() => PublicationValidator.Validate(_valid).Count;

    [Benchmark]
    public int ValidateInvalidManifest() => PublicationValidator.Validate(_invalid).Count;

    [Benchmark]
    public int ValidateHundredContentReferences() => PublicationValidator.Validate(_hundredItems).Count;
}
