using System.Security.Claims;
using CreatorNetwork.Contracts;
using TheSingularityWorkshop.Profiles;

namespace CreatorNetwork.Profiles;

/// <summary>
/// Coordinates trusted publisher resolution and safe identity projection for publication.
/// </summary>
/// <remarks>
/// This adapter requires a host-supplied resolver. It checks that the principal is authenticated
/// and that the audience is initialized, but authentication itself must be established by the host.
/// Resolver implementations own mapping trust, persona selection, and audience-specific disclosure.
/// </remarks>
public sealed class ProfilesCreatorIdentityPublicationAdapter
{
    private readonly ITrustedProfilesPublisherResolver _resolver;

    /// <summary>Creates the adapter with the host's trusted Profiles resolver.</summary>
    public ProfilesCreatorIdentityPublicationAdapter(ITrustedProfilesPublisherResolver resolver)
    {
        ArgumentNullException.ThrowIfNull(resolver);
        _resolver = resolver;
    }

    /// <summary>
    /// Resolves one approved publisher persona for an audience and projects it into the wire contract.
    /// </summary>
    /// <param name="authenticatedPublisher">The host-authenticated publisher principal.</param>
    /// <param name="publicationAudience">Observer identity used for disclosure decisions.</param>
    /// <param name="approvedProfileUri">Optional profile URI approved by host policy.</param>
    /// <returns>A compact CreatorNetwork identity.</returns>
    /// <exception cref="ArgumentNullException">The publisher principal is null.</exception>
    /// <exception cref="ArgumentException">The principal is unauthenticated or the audience ID is uninitialized.</exception>
    /// <exception cref="InvalidOperationException">No unique trusted publisher representation is available.</exception>
    public CreatorIdentity ResolveForPublication(
        ClaimsPrincipal authenticatedPublisher,
        ProfileId publicationAudience,
        string? approvedProfileUri = null)
    {
        ArgumentNullException.ThrowIfNull(authenticatedPublisher);

        if (authenticatedPublisher.Identity?.IsAuthenticated != true)
        {
            throw new ArgumentException(
                "The publisher principal must be authenticated by the host.",
                nameof(authenticatedPublisher));
        }

        if (publicationAudience.Value == Guid.Empty)
        {
            throw new ArgumentException(
                "The publication audience must have an initialized Profiles identity.",
                nameof(publicationAudience));
        }

        var representation = _resolver.ResolveApprovedRepresentation(
            authenticatedPublisher,
            publicationAudience);

        if (representation is null)
        {
            throw new InvalidOperationException(
                "No unique trusted publisher persona is available for this publication audience.");
        }

        return ProfilesCreatorIdentityMapper.MapApprovedRepresentation(
            representation,
            approvedProfileUri);
    }
}
