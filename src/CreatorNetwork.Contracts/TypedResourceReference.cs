namespace CreatorNetwork.Contracts;

/// <summary>
/// Provider-neutral reference to non-executable supporting content.
/// This is metadata only: it is not a byte store, delivery API, authorization grant, or executable MicroBundle.
/// </summary>
public sealed class TypedResourceReference
{
    /// <summary>Creates a reference to one finalized supporting resource.</summary>
    public TypedResourceReference(
        string resourceId,
        string resourceType,
        string role,
        string mediaType,
        string sha256,
        long? lengthBytes = null,
        string? sourceUri = null,
        string? rightsDeclarationId = null,
        string? provenanceRecordId = null)
    {
        ResourceId = resourceId ?? string.Empty;
        ResourceType = resourceType ?? string.Empty;
        Role = role ?? string.Empty;
        MediaType = mediaType ?? string.Empty;
        Sha256 = sha256 ?? string.Empty;
        LengthBytes = lengthBytes;
        SourceUri = sourceUri;
        RightsDeclarationId = rightsDeclarationId;
        ProvenanceRecordId = provenanceRecordId;
    }

    /// <summary>Stable resource identity, independent of a provider URL.</summary>
    public string ResourceId { get; }
    /// <summary>Extensible resource type identifier, such as urn:workshop:resource:image or urn:workshop:resource:dataset.</summary>
    public string ResourceType { get; }
    /// <summary>Publisher-declared role within the publication, such as thumbnail, transcript, or attachment.</summary>
    public string Role { get; }
    /// <summary>Media type of the finalized bytes.</summary>
    public string MediaType { get; }
    /// <summary>SHA-256 digest of the exact finalized resource bytes.</summary>
    public string Sha256 { get; }
    /// <summary>Optional known byte length.</summary>
    public long? LengthBytes { get; }
    /// <summary>Optional untrusted location hint; never identity or permission and must not be fetched blindly.</summary>
    public string? SourceUri { get; }
    /// <summary>Optional reference to the exact scoped rights declaration for this resource.</summary>
    public string? RightsDeclarationId { get; }
    /// <summary>Optional reference to the provenance record that produces this resource.</summary>
    public string? ProvenanceRecordId { get; }
}
