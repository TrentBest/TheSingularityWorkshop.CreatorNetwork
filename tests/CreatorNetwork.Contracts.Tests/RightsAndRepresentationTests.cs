using System;
using System.Collections.Generic;
using CreatorNetwork.Contracts;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class RightsAndRepresentationTests
{
    [Fact]
    public void ScopedRightsDeclarationAcceptsExplicitActions()
    {
        var declaration = new RightsDeclaration(
            "rights:museum:statue-low-poly:v1",
            "content:statue-low-poly:v1",
            "rights-holder:museum",
            new[] { RightsAction.Display, RightsAction.Embed },
            RightsDeclarationStatus.Asserted,
            "license:collection-external-use",
            "audience:public",
            "territory:worldwide",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow.AddYears(1));

        Assert.Empty(RightsDeclarationValidator.Validate(declaration));
        Assert.DoesNotContain(RightsAction.Transform, declaration.GrantedActions);
        Assert.DoesNotContain(RightsAction.CommercialUse, declaration.GrantedActions);
    }

    [Fact]
    public void RightsDeclarationRejectsDuplicateActionsAndInvalidWindow()
    {
        var start = DateTimeOffset.UtcNow;
        var declaration = new RightsDeclaration(
            "rights:bad", "content:one", "holder:one",
            new[] { RightsAction.Display, RightsAction.Display },
            validFrom: start,
            validUntil: start);

        IReadOnlyList<PublicationValidationError> errors = RightsDeclarationValidator.Validate(declaration);

        Assert.Contains(errors, x => x.Code == "rights.action.duplicate");
        Assert.Contains(errors, x => x.Code == "rights.validity.invalid");
    }

    [Fact]
    public void SourceApprovedAlternativeMustReferenceDistinctArtifactAndRights()
    {
        var offer = new RepresentationOffer(
            "offer:museum:gallery-centerpiece",
            "publisher:museum",
            "work:statue",
            "content:restricted-original",
            "content:approved-low-poly",
            "gallery.centerpiece",
            "provenance:low-poly",
            "rights:low-poly");

        Assert.Empty(RepresentationOfferValidator.Validate(offer));
    }

    [Fact]
    public void RepresentationOfferCannotUseSourceAsItsOwnAlternative()
    {
        var offer = new RepresentationOffer(
            "offer:self", "publisher:museum", "work:statue",
            "content:same", "content:same", "gallery.centerpiece",
            "provenance:one", "rights:one");

        Assert.Contains(RepresentationOfferValidator.Validate(offer),
            x => x.Code == "representation.alternative.same_as_source");
    }

    [Fact]
    public void RightsActionsAreSnapshotted()
    {
        var actions = new List<RightsAction> { RightsAction.Display };
        var declaration = new RightsDeclaration("rights:snapshot", "resource:one", "holder:one", actions);

        actions.Add(RightsAction.Download);

        Assert.Single(declaration.GrantedActions);
    }
}
