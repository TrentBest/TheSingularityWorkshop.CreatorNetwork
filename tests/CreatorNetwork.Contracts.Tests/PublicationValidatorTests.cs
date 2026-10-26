using System;
using System.Collections.Generic;
using CreatorNetwork.Contracts;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class PublicationValidatorTests
{
    [Fact]
    public void ValidManifestHasNoValidationErrors()
    {
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "pub:creator-123:channel-main",
            new CreatorIdentity("creator:123", "Ada"),
            "Research notes",
            "A portable channel.",
            "public",
            1,
            DateTimeOffset.UtcNow,
            new[]
            {
                new ContentReference("content:one", "video/mp4", 1024, new string('a', 64), "https://example.test/video.mp4")
            });

        Assert.Empty(PublicationValidator.Validate(publication));
    }

    [Fact]
    public void UnsupportedSchemaIsActionable()
    {
        var publication = ValidPublication(schema: "creator-network.channel/99");

        var error = Assert.Single(PublicationValidator.Validate(publication));

        Assert.Equal("schema.unsupported", error.Code);
        Assert.Equal("schema", error.Field);
    }

    [Fact]
    public void MultipleInvalidFieldsAreReportedTogether()
    {
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "",
            new CreatorIdentity(""),
            "",
            null,
            "world",
            0,
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)),
            new[] { new ContentReference("", "", -1, "not-a-digest") });

        IReadOnlyList<PublicationValidationError> errors = PublicationValidator.Validate(publication);

        Assert.Contains(errors, x => x.Code == "publication.id.required");
        Assert.Contains(errors, x => x.Code == "creator.id.required");
        Assert.Contains(errors, x => x.Code == "visibility.invalid");
        Assert.Contains(errors, x => x.Code == "timestamp.not_utc");
        Assert.Contains(errors, x => x.Code == "content.sha256.invalid");
    }

    [Fact]
    public void NullManifestProducesOneErrorRatherThanThrowing()
    {
        var error = Assert.Single(PublicationValidator.Validate(null));

        Assert.Equal("publication.required", error.Code);
    }

    private static ChannelPublication ValidPublication(string? schema = null) =>
        new(
            schema ?? ChannelPublication.CurrentSchema,
            "pub:creator-123:channel-main",
            new CreatorIdentity("creator:123"),
            "Research notes",
            null,
            "public",
            1,
            DateTimeOffset.UtcNow);
}
