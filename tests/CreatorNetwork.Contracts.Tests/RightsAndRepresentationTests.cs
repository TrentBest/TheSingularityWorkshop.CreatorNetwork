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
        Assert.DoesNotContain(RightsAction.Cache, declaration.GrantedActions);
        Assert.DoesNotContain(RightsAction.Replicate, declaration.GrantedActions);
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
    public void MuseumAlternativeRetainsItsOwnProvenanceAndRightsScope()
    {
        var sourceProvenance = new ProvenanceRecord(
            "provenance:scan", "content:original", "publisher:museum", "publisher:museum",
            "capture", DateTimeOffset.UtcNow,
            new[] { new ProvenanceReference(ProvenanceReferenceKind.RealWorldReferent, "object:statue") },
            originatorStatus: ProvenanceOriginatorStatus.Claimed);
        var alternativeProvenance = new ProvenanceRecord(
            "provenance:approved-low-poly", "content:low-poly", "publisher:museum", "publisher:museum",
            "authorized-representation", DateTimeOffset.UtcNow,
            new[] { new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:original") },
            new[] { "Reduced complexity using the publisher-approved workflow." },
            originatorStatus: ProvenanceOriginatorStatus.Claimed);
        var originalRights = new RightsDeclaration(
            "rights:original", "content:original", "rights-holder:museum",
            new[] { RightsAction.Display });
        var alternativeRights = new RightsDeclaration(
            "rights:low-poly", "content:low-poly", "rights-holder:museum",
            new[] { RightsAction.Display, RightsAction.Embed });
        var offer = new RepresentationOffer(
            "offer:statue", "publisher:museum", "work:statue",
            "content:original", "content:low-poly", "gallery.centerpiece",
            "provenance:approved-low-poly", "rights:low-poly");

        Assert.Empty(ProvenanceGraphValidator.Validate(new[] { sourceProvenance, alternativeProvenance }));
        Assert.Empty(RightsDeclarationValidator.Validate(originalRights));
        Assert.Empty(RightsDeclarationValidator.Validate(alternativeRights));
        Assert.Empty(RepresentationOfferValidator.Validate(offer));
        Assert.DoesNotContain(RightsAction.Embed, originalRights.GrantedActions);
        Assert.Contains(RightsAction.Embed, alternativeRights.GrantedActions);
        Assert.DoesNotContain(RightsAction.Transform, alternativeRights.GrantedActions);
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
