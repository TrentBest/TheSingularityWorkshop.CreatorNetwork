using System;

namespace CreatorNetwork.Contracts;

/// <summary>
/// Stable creator-owned collection metadata. A channel may exist without any publications.
/// This is the v2 channel contract; the v1 ChannelPublication DTO remains unchanged.
/// </summary>
public sealed class CreatorChannel
{
    /// <summary>The schema identifier for the separated channel contract.</summary>
    public const string CurrentSchema = "creator-network.channel/2";

    /// <summary>Creates a stable channel metadata snapshot.</summary>
    public CreatorChannel(
        string schema,
        string channelId,
        CreatorIdentity creator,
        string title,
        string? description,
        string visibility,
        DateTimeOffset createdAt,
        DateTimeOffset updatedAt)
    {
        Schema = schema ?? string.Empty;
        ChannelId = channelId ?? string.Empty;
        Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        Title = title ?? string.Empty;
        Description = description;
        Visibility = visibility ?? string.Empty;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    /// <summary>Versioned channel schema identifier.</summary>
    public string Schema { get; }
    /// <summary>Stable portable channel identity, independent of a provider account.</summary>
    public string ChannelId { get; }
    /// <summary>Creator identity projection approved for publication.</summary>
    public CreatorIdentity Creator { get; }
    /// <summary>Human-readable channel title.</summary>
    public string Title { get; }
    /// <summary>Optional channel description.</summary>
    public string? Description { get; }
    /// <summary>Declared channel visibility; trusted hosts enforce actual access.</summary>
    public string Visibility { get; }
    /// <summary>Creation instant expressed in UTC.</summary>
    public DateTimeOffset CreatedAt { get; }
    /// <summary>Last metadata update instant expressed in UTC.</summary>
    public DateTimeOffset UpdatedAt { get; }
}
