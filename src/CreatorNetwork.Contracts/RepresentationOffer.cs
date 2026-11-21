using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>
/// A source-publisher assertion that one alternate artifact may represent a subject in a semantic role.
/// It does not authorize use; the alternative has its own provenance and rights scope.
/// </summary>
public sealed class RepresentationOffer
{
    /// <summary>Creates a source-approved representation offer.</summary>
    public RepresentationOffer(
        string offerId,
        string publisherId,
        string subjectId,
        string sourceContentId,
        string alternativeContentId,
        string semanticRole,
        string provenanceRecordId,
        string rightsDeclarationId,
        string? compatibilityProfile = null)
    {
        OfferId = offerId ?? string.Empty;
        PublisherId = publisherId ?? string.Empty;
        SubjectId = subjectId ?? string.Empty;
        SourceContentId = sourceContentId ?? string.Empty;
        AlternativeContentId = alternativeContentId ?? string.Empty;
        SemanticRole = semanticRole ?? string.Empty;
        ProvenanceRecordId = provenanceRecordId ?? string.Empty;
        RightsDeclarationId = rightsDeclarationId ?? string.Empty;
        CompatibilityProfile = compatibilityProfile;
    }

    /// <summary>Stable identifier for this offer/version.</summary>
    public string OfferId { get; }
    /// <summary>Publisher asserting control over the substitution mapping; host verification remains external.</summary>
    public string PublisherId { get; }
    /// <summary>Logical subject/work represented by both source and alternative.</summary>
    public string SubjectId { get; }
    /// <summary>Exact source artifact for which this alternative is offered.</summary>
    public string SourceContentId { get; }
    /// <summary>Exact alternate artifact. It is a separate resource with separate rights.</summary>
    public string AlternativeContentId { get; }
    /// <summary>Semantic composition slot, such as gallery.centerpiece.</summary>
    public string SemanticRole { get; }
    /// <summary>Provenance record identifying how the alternative was produced.</summary>
    public string ProvenanceRecordId { get; }
    /// <summary>Rights declaration scoped to the alternative artifact.</summary>
    public string RightsDeclarationId { get; }
    /// <summary>Optional consumer-defined compatibility profile; not executable policy.</summary>
    public string? CompatibilityProfile { get; }
}

/// <summary>Structural validation for source-approved representation offers.</summary>
public static class RepresentationOfferValidator
{
    /// <summary>Validates required references; does not verify publisher authority, compatibility, or authorization.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(RepresentationOffer? offer)
    {
        if (offer is null)
            return new[] { new PublicationValidationError("representation.offer.required", "representation", "A representation offer is required.") };

        List<PublicationValidationError>? errors = null;
        if (string.IsNullOrWhiteSpace(offer.OfferId))
            Add(ref errors, "representation.offer_id.required", "offerId", "Provide a stable offer identifier.");
        if (string.IsNullOrWhiteSpace(offer.PublisherId))
            Add(ref errors, "representation.publisher.required", "publisherId", "Identify the source publisher asserting this offer.");
        if (string.IsNullOrWhiteSpace(offer.SubjectId))
            Add(ref errors, "representation.subject.required", "subjectId", "Identify the logical subject/work represented.");
        if (string.IsNullOrWhiteSpace(offer.SourceContentId))
            Add(ref errors, "representation.source.required", "sourceContentId", "Identify the exact source artifact.");
        if (string.IsNullOrWhiteSpace(offer.AlternativeContentId))
            Add(ref errors, "representation.alternative.required", "alternativeContentId", "Identify the exact alternative artifact.");
        if (!string.IsNullOrWhiteSpace(offer.SourceContentId) &&
            string.Equals(offer.SourceContentId, offer.AlternativeContentId, StringComparison.Ordinal))
            Add(ref errors, "representation.alternative.same_as_source", "alternativeContentId", "An alternative must have a distinct artifact identity.");
        if (string.IsNullOrWhiteSpace(offer.SemanticRole))
            Add(ref errors, "representation.role.required", "semanticRole", "Identify the semantic composition role.");
        if (string.IsNullOrWhiteSpace(offer.ProvenanceRecordId))
            Add(ref errors, "representation.provenance.required", "provenanceRecordId", "Reference the alternative's provenance record.");
        if (string.IsNullOrWhiteSpace(offer.RightsDeclarationId))
            Add(ref errors, "representation.rights.required", "rightsDeclarationId", "Reference a rights declaration scoped to the alternative artifact.");

        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    /// <summary>
    /// Validates that an offer points to the exact alternate output and its own rights scope.
    /// This is reference consistency only; the host must still verify publisher authority and authorize use.
    /// </summary>
    public static IReadOnlyList<PublicationValidationError> Validate(
        RepresentationOffer? offer,
        ProvenanceRecord? provenance,
        RightsDeclaration? rights)
    {
        IReadOnlyList<PublicationValidationError> basic = Validate(offer);
        var errors = new List<PublicationValidationError>(basic);
        if (offer is null)
            return errors;

        if (provenance is null)
        {
            errors.Add(new PublicationValidationError("representation.provenance.missing", "provenanceRecord", "The referenced provenance record must be supplied."));
        }
        else
        {
            if (!string.Equals(provenance.RecordId, offer.ProvenanceRecordId, StringComparison.Ordinal))
                errors.Add(new PublicationValidationError("representation.provenance.mismatch", "provenanceRecordId", "The supplied provenance record does not match the offer reference."));
            if (!string.Equals(provenance.OutputContentId, offer.AlternativeContentId, StringComparison.Ordinal))
                errors.Add(new PublicationValidationError("representation.provenance.output_mismatch", "alternativeContentId", "The provenance record must describe the exact alternative artifact."));
            bool sourceIsInput = false;
            for (int i = 0; i < provenance.Inputs.Count; i++)
            {
                ProvenanceReference? input = provenance.Inputs[i];
                if (input is not null && input.Kind == ProvenanceReferenceKind.ContentArtifact &&
                    string.Equals(input.Id, offer.SourceContentId, StringComparison.Ordinal))
                {
                    sourceIsInput = true;
                    break;
                }
            }
            if (!sourceIsInput)
                errors.Add(new PublicationValidationError("representation.provenance.source_missing", "sourceContentId", "The alternative provenance must identify the offered source artifact as an input."));
        }

        if (rights is null)
        {
            errors.Add(new PublicationValidationError("representation.rights.missing", "rightsDeclaration", "The referenced rights declaration must be supplied."));
        }
        else
        {
            if (!string.Equals(rights.DeclarationId, offer.RightsDeclarationId, StringComparison.Ordinal))
                errors.Add(new PublicationValidationError("representation.rights.mismatch", "rightsDeclarationId", "The supplied rights declaration does not match the offer reference."));
            if (!string.Equals(rights.ResourceId, offer.AlternativeContentId, StringComparison.Ordinal))
                errors.Add(new PublicationValidationError("representation.rights.resource_mismatch", "alternativeContentId", "The rights declaration must be scoped to the exact alternative artifact."));
            if (rights.Status != RightsDeclarationStatus.Asserted)
                errors.Add(new PublicationValidationError("representation.rights.not_active", "rightsDeclarationId", "A disputed or withdrawn rights declaration cannot support an active representation offer."));
        }

        return errors;
    }

    private static void Add(ref List<PublicationValidationError>? errors, string code, string field, string message)
    {
        errors ??= new List<PublicationValidationError>(4);
        errors.Add(new PublicationValidationError(code, field, message));
    }
}
