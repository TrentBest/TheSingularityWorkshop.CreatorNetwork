using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>A versioned, portable channel manifest. This type contains metadata, not media bytes.</summary>
public sealed class ChannelPublication
{
    /// <summary>The schema identifier for the first supported channel-publication contract.</summary>
    public const string CurrentSchema = "creator-network.channel/1";

    /// <summary>Creates a channel publication. Call <see cref="PublicationValidator.Validate(ChannelPublication)"/> before accepting untrusted data.</summary>
    public ChannelPublication(
        string schema,
        string publicationId,
        CreatorIdentity creator,
        string title,
        string? description,
        string visibility,
        long revision,
        DateTimeOffset updatedAt,
        IReadOnlyList<ContentReference>? content = null,
        string? attribution = null)
    {
        Schema = schema ?? string.Empty;
        PublicationId = publicationId ?? string.Empty;
        Creator = creator ?? throw new ArgumentNullException(nameof(creator));
        Title = title ?? string.Empty;
        Description = description;
        Visibility = visibility ?? string.Empty;
        Revision = revision;
        UpdatedAt = updatedAt;
        Content = content ?? Array.Empty<ContentReference>();
        Attribution = attribution;
    }

    /// <summary>Versioned schema identifier; unknown versions must be rejected or explicitly handled.</summary>
    public string Schema { get; }
    /// <summary>Stable publication identifier across host migration.</summary>
    public string PublicationId { get; }
    /// <summary>Portable creator identity reference.</summary>
    public CreatorIdentity Creator { get; }
    /// <summary>Human-readable channel title.</summary>
    public string Title { get; }
    /// <summary>Optional channel description.</summary>
    public string? Description { get; }
    /// <summary>Declared visibility policy: public, unlisted, or private. Enforcement belongs to the host.</summary>
    public string Visibility { get; }
    /// <summary>Positive, creator-assigned revision number.</summary>
    public long Revision { get; }
    /// <summary>Timestamp for this revision in UTC.</summary>
    public DateTimeOffset UpdatedAt { get; }
    /// <summary>Content references included in the channel, in display order.</summary>
    public IReadOnlyList<ContentReference> Content { get; }
    /// <summary>Optional attribution or rights statement; not a substitute for rights verification.</summary>
    public string? Attribution { get; }
}
