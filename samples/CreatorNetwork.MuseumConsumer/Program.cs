using System;
using System.Collections.Generic;
using System.Linq;
using CreatorNetwork.Contracts;

// This is a local, in-memory consumer demonstration. The allow-list below stands in for
// a real host's authenticated publisher/rights-holder verification and authorization service.
var now = DateTimeOffset.UtcNow;
var source = new ProvenanceRecord(
    "provenance:museum:scan-v1",
    "content:museum:statue-original-v1",
    "publisher:museum",
    "rights-holder:museum",
    "capture",
    now,
    new[] { new ProvenanceReference(ProvenanceReferenceKind.RealWorldReferent, "object:museum:statue") },
    new[] { "Captured the museum-held sculpture as a source artifact." },
    "Museum capture workflow",
    "1.0",
    ProvenanceOriginatorStatus.Claimed);

var approvedAlternative = new ProvenanceRecord(
    "provenance:museum:approved-low-poly-v1",
    "content:museum:statue-low-poly-v1",
    "publisher:museum",
    "rights-holder:museum",
    "authorized-representation",
    now,
    new[]
    {
        new ProvenanceReference(ProvenanceReferenceKind.DigitalWork, "work:museum:statue"),
        new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:museum:statue-original-v1")
    },
    new[] { "Created the museum-approved low-complexity representation." },
    "Museum representation workflow",
    "2.1",
    ProvenanceOriginatorStatus.Claimed);

var originalRights = new RightsDeclaration(
    "rights:museum:statue-original-v1",
    "content:museum:statue-original-v1",
    "rights-holder:museum",
    new[] { RightsAction.Display });

var alternativeRights = new RightsDeclaration(
    "rights:museum:statue-low-poly-v1",
    "content:museum:statue-low-poly-v1",
    "rights-holder:museum",
    new[] { RightsAction.Display, RightsAction.Embed },
    RightsDeclarationStatus.Asserted,
    "license:museum:approved-external-representation",
    "audience:public",
    "territory:worldwide",
    now.AddMinutes(-1),
    now.AddDays(30));

var offer = new RepresentationOffer(
    "offer:museum:gallery-centerpiece:v1",
    "publisher:museum",
    "work:museum:statue",
    "content:museum:statue-original-v1",
    "content:museum:statue-low-poly-v1",
    "gallery.centerpiece",
    "provenance:museum:approved-low-poly-v1",
    "rights:museum:statue-low-poly-v1",
    "mesh.static.low-poly");

var errors = new List<PublicationValidationError>();
errors.AddRange(ProvenanceGraphValidator.Validate(new[] { source, approvedAlternative }));
errors.AddRange(RightsDeclarationValidator.Validate(originalRights));
errors.AddRange(RightsDeclarationValidator.Validate(alternativeRights));
errors.AddRange(RepresentationOfferValidator.Validate(offer, approvedAlternative, alternativeRights));

if (errors.Count > 0)
{
    foreach (PublicationValidationError error in errors)
        Console.Error.WriteLine($"{error.Code} ({error.Field}): {error.Message}");
    return 1;
}

const string trustedPublisher = "publisher:museum";
const string trustedRightsHolder = "rights-holder:museum";
const string requestedAudience = "audience:public";
const RightsAction requestedAction = RightsAction.Embed;

bool hostTrustsPublisher =
    string.Equals(offer.PublisherId, trustedPublisher, StringComparison.Ordinal) &&
    string.Equals(alternativeRights.RightsHolderId, trustedRightsHolder, StringComparison.Ordinal);
bool inValidityWindow =
    (!alternativeRights.ValidFrom.HasValue || now >= alternativeRights.ValidFrom.Value) &&
    (!alternativeRights.ValidUntil.HasValue || now < alternativeRights.ValidUntil.Value);
bool audienceMatches =
    string.IsNullOrEmpty(alternativeRights.AudienceScope) ||
    string.Equals(alternativeRights.AudienceScope, requestedAudience, StringComparison.Ordinal);
bool actionGranted =
    alternativeRights.Status == RightsDeclarationStatus.Asserted &&
    alternativeRights.GrantedActions.Contains(requestedAction) &&
    inValidityWindow &&
    audienceMatches;

if (!hostTrustsPublisher || !actionGranted)
{
    Console.Error.WriteLine("The host did not authorize this representation for the requested action.");
    return 2;
}

bool originalMayBeEmbedded = originalRights.GrantedActions.Contains(requestedAction);
if (originalMayBeEmbedded)
{
    Console.Error.WriteLine("The fixture is invalid: the original unexpectedly grants embedding.");
    return 3;
}

Console.WriteLine("Museum representation contract validated.");
Console.WriteLine($"Semantic role: {offer.SemanticRole}");
Console.WriteLine($"Original may be embedded: {originalMayBeEmbedded}");
Console.WriteLine($"Selected source-approved alternative: {offer.AlternativeContentId}");
Console.WriteLine("The consumer did not fetch or transform the restricted original.");
Console.WriteLine("Host trust checks in this sample are illustrative, not production identity verification.");
return 0;
