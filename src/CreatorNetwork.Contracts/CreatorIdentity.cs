namespace CreatorNetwork.Contracts;

/// <summary>
/// Portable, publication-safe reference to the creator identity intentionally presented by a channel.
/// This is a wire-contract projection, not a Profiles Profile or ProfileRepresentation and not proof
/// of authentication. Hosts should map an approved Profiles representation into this contract.
/// </summary>
public sealed class CreatorIdentity
{
    /// <summary>Creates a portable reference to the intentionally published creator identity.</summary>
    public CreatorIdentity(string id, string? displayName = null, string? profileUri = null)
    {
        Id = id ?? string.Empty;
        DisplayName = displayName;
        ProfileUri = profileUri;
    }

    /// <summary>Stable opaque identifier for the published persona; it must not be a host-local username.</summary>
    public string Id { get; }
    /// <summary>Optional display name explicitly approved for this publication; not an identity key or proof.</summary>
    public string? DisplayName { get; }
    /// <summary>Optional profile URI/location hint; not the identity itself and not authentication proof.</summary>
    public string? ProfileUri { get; }
}
