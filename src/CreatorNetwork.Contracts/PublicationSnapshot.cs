using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>
/// Immutable, versioned publication snapshot within a stable creator-owned channel.
/// Channel metadata changes do not rewrite this snapshot; publishing an update creates a new revision.
/// </summary>
public sealed class PublicationSnapshot
{
    /// <summary>The schema identifier for the separated publication contract.</summary>
    public const string CurrentSchema = "creator-network.publication/2";

    /// <summary>Creates an immutable publication snapshot.</summary>
    public PublicationSnapshot(
        string schema,
        string publicationId,
        string channelId,
        long revision,
        string title,
        string? description,
        string visibility,
        DateTimeOffset publishedAt,
        ExperienceArtifactReference primaryExperience,
        IReadOnlyList<TypedResourceReference>? resources = null)
    {
        Schema = schema ?? string.Empty;
        PublicationId = publicationId ?? string.Empty;
        ChannelId = channelId ?? string.Empty;
        Revision = revision;
        Title = title ?? string.Empty;
        Description = description;
        Visibility = visibility ?? string.Empty;
        PublishedAt = publishedAt;
        PrimaryExperience = primaryExperience ?? throw new ArgumentNullException(nameof(primaryExperience));
        Resources = Snapshot(resources);
    }

    private static IReadOnlyList<TypedResourceReference> Snapshot(IReadOnlyList<TypedResourceReference>? source)
    {
        if (source is null || source.Count == 0)
            return Array.Empty<TypedResourceReference>();

        var copy = new TypedResourceReference[source.Count];
        for (int i = 0; i < copy.Length; i++)
            copy[i] = source[i];
        return Array.AsReadOnly(copy);
    }

    /// <summary>Versioned publication schema identifier.</summary>
    public string Schema { get; }
    /// <summary>Stable publication identity across revisions and provider migration.</summary>
    public string PublicationId { get; }
    /// <summary>Stable channel identity containing this publication.</summary>
    public string ChannelId { get; }
    /// <summary>Positive monotonically increasing revision within the publication identity.</summary>
    public long Revision { get; }
    /// <summary>Publication-time title snapshot.</summary>
    public string Title { get; }
    /// <summary>Publication-time description snapshot.</summary>
    public string? Description { get; }
    /// <summary>Declared publication visibility; enforcement belongs to the trusted host/provider.</summary>
    public string Visibility { get; }
    /// <summary>Publication instant expressed in UTC.</summary>
    public DateTimeOffset PublishedAt { get; }
    /// <summary>Primary finalized Experience artifact reference; bytes remain owned by its repository/provider.</summary>
    public ExperienceArtifactReference PrimaryExperience { get; }
    /// <summary>Ordered references to non-executable supporting resources. The list is a defensive snapshot.</summary>
    public IReadOnlyList<TypedResourceReference> Resources { get; }
}
