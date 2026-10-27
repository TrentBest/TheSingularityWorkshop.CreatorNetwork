using System.Security.Claims;
using TheSingularityWorkshop.Profiles;

namespace CreatorNetwork.Profiles;

/// <summary>
/// Host-owned trust boundary that resolves an authenticated publisher to the intended
/// publication persona and applies disclosure rules for the supplied audience.
/// </summary>
/// <remarks>
/// Implementations must reject missing, ambiguous, or untrusted mappings by returning
/// <see langword="null"/>. They must not infer trust from display names, profile URIs,
/// or external identity references. The returned representation must be audience-appropriate.
/// </remarks>
public interface ITrustedProfilesPublisherResolver
{
    /// <summary>
    /// Resolves the authenticated publisher to one approved persona representation for an audience.
    /// </summary>
    /// <param name="authenticatedPublisher">An authenticated principal established by the host.</param>
    /// <param name="publicationAudience">The observer identity whose disclosure policy applies.</param>
    /// <returns>The approved representation, or <see langword="null"/> if resolution is not unambiguous and trusted.</returns>
    ProfileRepresentation? ResolveApprovedRepresentation(
        ClaimsPrincipal authenticatedPublisher,
        ProfileId publicationAudience);
}
