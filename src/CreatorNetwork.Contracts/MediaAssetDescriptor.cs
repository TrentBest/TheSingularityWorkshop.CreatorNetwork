namespace CreatorNetwork.Contracts;

/// <summary>
/// Describes a finalized, durable media asset. A descriptor is not an authorization token
/// and does not imply that a particular host is permitted to serve the asset.
/// </summary>
public sealed class MediaAssetDescriptor
{
    public MediaAssetDescriptor(string assetId, string mediaType, long lengthBytes, string sha256)
    {
        AssetId = assetId ?? string.Empty;
        MediaType = mediaType ?? string.Empty;
        LengthBytes = lengthBytes;
        Sha256 = sha256 ?? string.Empty;
    }

    /// <summary>Stable opaque identifier for the asset, independent of its current host.</summary>
    public string AssetId { get; }

    /// <summary>Declared media type of the finalized bytes.</summary>
    public string MediaType { get; }

    /// <summary>Actual number of bytes committed to storage.</summary>
    public long LengthBytes { get; }

    /// <summary>Lowercase SHA-256 digest computed from the bytes committed to storage.</summary>
    public string Sha256 { get; }
}
