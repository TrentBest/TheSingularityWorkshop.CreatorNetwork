using CreatorNetwork.Contracts;
using TheSingularityWorkshop.Profiles;

namespace CreatorNetwork.Profiles;

/// <summary>
/// Projects an already approved, observer-appropriate Profiles representation into
/// CreatorNetwork's deliberately small publication identity contract.
/// </summary>
/// <remarks>
/// This mapper is not an authentication or authorization boundary. The caller must resolve
/// the authenticated publisher to the intended public persona, apply Profiles disclosure
/// rules for the publication audience, and pass only that approved representation. The
/// optional profile URI must also be approved by trusted host policy. Claims and avatars are
/// deliberately not copied into the wire identity.
/// </remarks>
public static class ProfilesCreatorIdentityMapper
{
    /// <summary>
    /// Maps a representation that the host has already approved for this publication.
    /// </summary>
    /// <param name="approvedRepresentation">
    /// The observer-appropriate representation selected by trusted host integration.
    /// </param>
    /// <param name="approvedProfileUri">
    /// Optional profile location explicitly approved by the host; never treated as identity proof.
    /// </param>
    /// <returns>A portable CreatorNetwork identity containing only the supported identity fields.</returns>
    /// <exception cref="ArgumentNullException">
    /// The representation is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The representation has an uninitialized profile identifier.
    /// </exception>
    public static CreatorIdentity MapApprovedRepresentation(
        ProfileRepresentation approvedRepresentation,
        string? approvedProfileUri = null)
    {
        ArgumentNullException.ThrowIfNull(approvedRepresentation);

        if (approvedRepresentation.Id.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "The approved representation must have an initialized Profiles identity.",
                nameof(approvedRepresentation));
        }

        return new CreatorIdentity(
            approvedRepresentation.Id.ToString(),
            approvedRepresentation.DisplayName,
            approvedProfileUri);
    }
}
