namespace CreatorNetwork.Contracts;

/// <summary>
/// Identifies the kind of upstream object named by a provenance reference.
/// A reference identifies lineage; it does not prove ownership, authenticity, or permission.
/// </summary>
public enum ProvenanceReferenceKind
{
    /// <summary>A real-world person, object, place, work, event, or other subject represented by content.</summary>
    RealWorldReferent = 0,
    /// <summary>A logical creative work, distinct from any particular digital file containing it.</summary>
    DigitalWork = 1,
    /// <summary>A particular digital content artifact or version.</summary>
    ContentArtifact = 2,
    /// <summary>Evidence or an external record relevant to the asserted lineage.</summary>
    ExternalEvidence = 3
}

/// <summary>A typed, stable reference to an upstream subject, work, artifact, or evidence item.</summary>
public sealed class ProvenanceReference
{
    /// <summary>Creates a provenance reference.</summary>
    public ProvenanceReference(ProvenanceReferenceKind kind, string id)
    {
        Kind = kind;
        Id = id ?? string.Empty;
    }

    /// <summary>The semantic kind of referenced item.</summary>
    public ProvenanceReferenceKind Kind { get; }

    /// <summary>Stable opaque identifier; never interpret it as authorization.</summary>
    public string Id { get; }
}
