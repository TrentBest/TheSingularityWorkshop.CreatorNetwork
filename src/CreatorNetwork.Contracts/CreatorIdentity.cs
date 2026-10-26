namespace CreatorNetwork.Contracts;

/// <summary>A portable creator identity reference, independent of any hosting account.</summary>
public sealed class CreatorIdentity
{
    /// <summary>Creates a creator identity reference.</summary>
    public CreatorIdentity(string id, string? displayName = null, string? profileUri = null)
    {
        Id = id ?? string.Empty;
        DisplayName = displayName;
        ProfileUri = profileUri;
    }

    /// <summary>Stable opaque identifier. It must not be a host-local username.</summary>
    public string Id { get; }
    /// <summary>Optional human-readable name; not an identity key.</summary>
    public string? DisplayName { get; }
    /// <summary>Optional profile URI. Consumers must not treat it as the sole identity.</summary>
    public string? ProfileUri { get; }
}
