namespace CreatorNetwork.Contracts;

/// <summary>Provider-neutral reference to content bytes; metadata does not imply byte portability.</summary>
public sealed class ContentReference
{
    /// <summary>Creates a reference to one logical content item.</summary>
    public ContentReference(string id, string mediaType, long? lengthBytes = null, string? sha256 = null, string? sourceUri = null)
    {
        Id = id ?? string.Empty;
        MediaType = mediaType ?? string.Empty;
        LengthBytes = lengthBytes;
        Sha256 = sha256;
        SourceUri = sourceUri;
    }

    /// <summary>Stable opaque content identifier, independent of a delivery URL.</summary>
    public string Id { get; }
    /// <summary>Media type such as video/mp4.</summary>
    public string MediaType { get; }
    /// <summary>Optional known byte length.</summary>
    public long? LengthBytes { get; }
    /// <summary>Optional lowercase or uppercase hexadecimal SHA-256 digest of the exact bytes.</summary>
    public string? Sha256 { get; }
    /// <summary>Optional currently usable location. This is a location hint, not the content identity.</summary>
    public string? SourceUri { get; }
}
