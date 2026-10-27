using System.Security.Claims;
using CreatorNetwork.Contracts;
using CreatorNetwork.Profiles;
using TheSingularityWorkshop.Profiles;
using Xunit;

namespace CreatorNetwork.Profiles.Tests;

public sealed class ProfilesCreatorIdentityPublicationAdapterTests
{
    [Fact]
    public void RequiresAuthenticatedPublisherBeforeCallingResolver()
    {
        var resolver = new StubResolver(null);
        var adapter = new ProfilesCreatorIdentityPublicationAdapter(resolver);
        var unauthenticated = new ClaimsPrincipal(new ClaimsIdentity());

        Assert.Throws<ArgumentException>(() =>
            adapter.ResolveForPublication(unauthenticated, ProfileId.New()));
        Assert.False(resolver.WasCalled);
    }

    [Fact]
    public void RequiresInitializedAudienceBeforeCallingResolver()
    {
        var resolver = new StubResolver(null);
        var adapter = new ProfilesCreatorIdentityPublicationAdapter(resolver);

        Assert.Throws<ArgumentException>(() =>
            adapter.ResolveForPublication(AuthenticatedPublisher(), default));
        Assert.False(resolver.WasCalled);
    }

    [Fact]
    public void FailsClosedWhenResolverCannotProvideOneTrustedRepresentation()
    {
        var resolver = new StubResolver(null);
        var adapter = new ProfilesCreatorIdentityPublicationAdapter(resolver);

        Assert.Throws<InvalidOperationException>(() =>
            adapter.ResolveForPublication(AuthenticatedPublisher(), ProfileId.New()));
    }

    [Fact]
    public void DelegatesAudienceAndMapsOnlyApprovedRepresentation()
    {
        var expectedAudience = ProfileId.New();
        var expectedRepresentation = new ProfileRepresentation(
            ProfileId.New(),
            "Publisher persona",
            "avatar-not-copied",
            new Dictionary<string, object?> { ["private-email"] = "private@example.test" });
        var resolver = new StubResolver(expectedRepresentation);
        var adapter = new ProfilesCreatorIdentityPublicationAdapter(resolver);

        CreatorIdentity result = adapter.ResolveForPublication(
            AuthenticatedPublisher(),
            expectedAudience,
            "https://example.test/publisher");

        Assert.Equal(expectedAudience, resolver.ReceivedAudience);
        Assert.Equal(expectedRepresentation.Id.ToString(), result.Id);
        Assert.Equal("Publisher persona", result.DisplayName);
        Assert.Equal("https://example.test/publisher", result.ProfileUri);
        Assert.DoesNotContain("private-email", result.ToString(), StringComparison.OrdinalIgnoreCase);
    }

    private static ClaimsPrincipal AuthenticatedPublisher() =>
        new(new ClaimsIdentity(
            new[] { new Claim(ClaimTypes.NameIdentifier, "host-subject-1") },
            authenticationType: "test-authentication"));

    private sealed class StubResolver(ProfileRepresentation? representation)
        : ITrustedProfilesPublisherResolver
    {
        public bool WasCalled { get; private set; }
        public ProfileId ReceivedAudience { get; private set; }

        public ProfileRepresentation? ResolveApprovedRepresentation(
            ClaimsPrincipal authenticatedPublisher,
            ProfileId publicationAudience)
        {
            WasCalled = true;
            ReceivedAudience = publicationAudience;
            return representation;
        }
    }
}
