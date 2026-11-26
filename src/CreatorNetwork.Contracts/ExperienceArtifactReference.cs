namespace CreatorNetwork.Contracts;

/// <summary>
/// Provider-neutral reference to an immutable Experiences artifact.
/// The tuple mirrors the existing Experience artifact address (numeric identity, version, SHA-256)
/// without importing or redefining MicroBundleRepository storage or transport behavior.
/// </summary>
public sealed class ExperienceArtifactReference
{
    /// <summary>Creates a reference to one finalized Experience artifact.</summary>
    public ExperienceArtifactReference(ulong experienceId, string version, string contentHash)
    {
        ExperienceId = experienceId;
        Version = version ?? string.Empty;
        ContentHash = contentHash ?? string.Empty;
    }

    /// <summary>Stable numeric Experience identity assigned by the Experiences domain.</summary>
    public ulong ExperienceId { get; }
    /// <summary>Explicit immutable artifact version.</summary>
    public string Version { get; }
    /// <summary>SHA-256 digest of the exact Experience artifact bytes.</summary>
    public string ContentHash { get; }
}
