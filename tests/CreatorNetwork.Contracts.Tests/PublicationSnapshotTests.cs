using System;
using System.Collections.Generic;
using CreatorNetwork.Contracts;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class PublicationSnapshotTests
{
    private static readonly DateTimeOffset PublishedAt =
        new(2026, 11, 26, 0, 0, 0, TimeSpan.Zero);

    [Fact]
    public void ChannelCanExistWithoutAnyPublications()
    {
        var channel = ValidChannel();

        Assert.Empty(CreatorChannelValidator.Validate(channel));
        // Publications are separate immutable records; an empty channel needs no placeholder publication.
        Assert.Equal("channel:creator:studio", channel.ChannelId);
    }

    [Fact]
    public void MultiplePublicationSnapshotsCanBelongToOneStableChannel()
    {
        var first = ValidPublication(revision: 1, title: "First release");
        var second = ValidPublication(revision: 2, title: "Second release");

        Assert.Empty(PublicationSnapshotValidator.Validate(first));
        Assert.Empty(PublicationSnapshotValidator.Validate(second));
        Assert.Equal(first.ChannelId, second.ChannelId);
        Assert.Equal(first.PublicationId, second.PublicationId);
        Assert.NotEqual(first.Revision, second.Revision);
    }

    [Fact]
    public void PublicationSnapshotDoesNotChangeWhenCallerMutatesResourceList()
    {
        var resources = new List<TypedResourceReference> { ValidResource("resource:poster") };
        var publication = ValidPublication(resources: resources);

        resources.Add(ValidResource("resource:transcript"));

        Assert.Single(publication.Resources);
        Assert.Equal("resource:poster", publication.Resources[0].ResourceId);
    }

    [Fact]
    public void ExperienceAddressIsPortableAndContainsNoProviderLocation()
    {
        var address = ValidExperience();
        var firstProviderCopy = new ExperienceArtifactReference(address.ExperienceId, address.Version, address.ContentHash);
        var secondProviderCopy = new ExperienceArtifactReference(address.ExperienceId, address.Version, address.ContentHash);

        Assert.Equal(firstProviderCopy.ExperienceId, secondProviderCopy.ExperienceId);
        Assert.Equal(firstProviderCopy.Version, secondProviderCopy.Version);
        Assert.Equal(firstProviderCopy.ContentHash, secondProviderCopy.ContentHash);
        Assert.Empty(PublicationSnapshotValidator.Validate(ValidPublication()));
    }

    [Fact]
    public void NonExecutableResourceMustHaveTypeRoleMediaTypeAndDigest()
    {
        var resource = new TypedResourceReference("resource:poster", "", "", "", "bad-digest");
        var publication = ValidPublication(resources: new[] { resource });

        var errors = PublicationSnapshotValidator.Validate(publication);

        Assert.Contains(errors, error => error.Code == "resource.type.required");
        Assert.Contains(errors, error => error.Code == "resource.role.required");
        Assert.Contains(errors, error => error.Code == "resource.media_type.required");
        Assert.Contains(errors, error => error.Code == "resource.sha256.invalid");
    }

    [Fact]
    public void DuplicateResourceIdentityIsRejected()
    {
        var publication = ValidPublication(resources: new[]
        {
            ValidResource("resource:duplicate"),
            ValidResource("resource:duplicate")
        });

        Assert.Contains(PublicationSnapshotValidator.Validate(publication),
            error => error.Code == "resource.id.duplicate");
    }

    [Fact]
    public void UnsafeExperienceVersionSegmentIsRejected()
    {
        var publication = ValidPublication(
            experience: new ExperienceArtifactReference(17, "../latest", new string('a', 64)));

        Assert.Contains(PublicationSnapshotValidator.Validate(publication),
            error => error.Code == "experience.version.invalid");
    }

    [Fact]
    public void ExistingV1ChannelContractRemainsValid()
    {
        var v1 = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "publication:v1",
            new CreatorIdentity("creator:studio"),
            "Legacy channel",
            null,
            "public",
            1,
            PublishedAt);

        Assert.Empty(PublicationValidator.Validate(v1));
    }

    private static CreatorChannel ValidChannel() =>
        new(
            CreatorChannel.CurrentSchema,
            "channel:creator:studio",
            new CreatorIdentity("creator:studio", "Studio"),
            "Studio",
            "An empty channel is a valid collection.",
            "public",
            PublishedAt,
            PublishedAt);

    private static PublicationSnapshot ValidPublication(
        long revision = 1,
        string title = "Museum exhibit",
        ExperienceArtifactReference? experience = null,
        IReadOnlyList<TypedResourceReference>? resources = null) =>
        new(
            PublicationSnapshot.CurrentSchema,
            "publication:museum:statue",
            "channel:creator:studio",
            revision,
            title,
            "An immutable museum Experience release.",
            "public",
            PublishedAt,
            experience ?? ValidExperience(),
            resources);

    private static ExperienceArtifactReference ValidExperience() =>
        new(17, "2.1.0", new string('a', 64));

    private static TypedResourceReference ValidResource(string id) =>
        new(
            id,
            "urn:workshop:resource:image",
            "exhibit.thumbnail",
            "image/png",
            new string('b', 64),
            4096,
            "https://provider.example.test/resources/poster.png",
            "rights:museum:poster",
            "provenance:museum:poster");
}
