using CreatorNetwork.Profiles;
using TheSingularityWorkshop.Profiles;
using Xunit;

namespace CreatorNetwork.Profiles.Tests;

public sealed class ProfilesCreatorIdentityMapperTests
{
    [Fact]
    public void MapsOnlyIdentityFieldsFromApprovedRepresentation()
    {
        var profileId = ProfileId.New();
        var representation = new ProfileRepresentation(
            profileId,
            "Public persona",
            "https://example.test/private-avatar-not-copied.png",
            new Dictionary<string, object?>
            {
                ["public-topic"] = "research",
                ["private-email"] = "private@example.test"
            });

        var identity = ProfilesCreatorIdentityMapper.MapApprovedRepresentation(
            representation,
            "https://example.test/persona");

        Assert.Equal(profileId.ToString(), identity.Id);
        Assert.Equal("Public persona", identity.DisplayName);
        Assert.Equal("https://example.test/persona", identity.ProfileUri);

        var propertyNames = typeof(CreatorNetwork.Contracts.CreatorIdentity)
            .GetProperties()
            .Select(property => property.Name)
            .OrderBy(name => name)
            .ToArray();

        Assert.Equal(new[] { "DisplayName", "Id", "ProfileUri" }, propertyNames);
    }

    [Fact]
    public void DoesNotInventOptionalPublicFields()
    {
        var representation = new ProfileRepresentation(
            ProfileId.New(),
            null,
            "https://example.test/avatar.png",
            new Dictionary<string, object?> { ["private"] = "must not be copied" });

        var identity = ProfilesCreatorIdentityMapper.MapApprovedRepresentation(representation);

        Assert.Null(identity.DisplayName);
        Assert.Null(identity.ProfileUri);
    }

    [Fact]
    public void RejectsUninitializedProfileIdentity()
    {
        var representation = new ProfileRepresentation(
            default,
            "Untrusted",
            null,
            new Dictionary<string, object?>());

        var exception = Assert.Throws<ArgumentException>(
            () => ProfilesCreatorIdentityMapper.MapApprovedRepresentation(representation));

        Assert.Contains("initialized", exception.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void RejectsNullRepresentation()
    {
        Assert.Throws<ArgumentNullException>(
            () => ProfilesCreatorIdentityMapper.MapApprovedRepresentation(null!));
    }
}
