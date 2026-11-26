using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>Structural validation for immutable publication snapshots.</summary>
public static class PublicationSnapshotValidator
{
    /// <summary>Validates snapshot structure; does not resolve providers, verify rights, or authorize access.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(PublicationSnapshot? publication)
    {
        if (publication is null)
            return new[] { new PublicationValidationError("publication.required", "$", "A publication snapshot is required.") };

        List<PublicationValidationError>? errors = null;
        if (!string.Equals(publication.Schema, PublicationSnapshot.CurrentSchema, StringComparison.Ordinal))
            Add(ref errors, "schema.unsupported", "schema", $"Unsupported schema '{publication.Schema}'. Expected '{PublicationSnapshot.CurrentSchema}'.");
        if (string.IsNullOrWhiteSpace(publication.PublicationId))
            Add(ref errors, "publication.id.required", "publicationId", "Provide a stable publication identifier.");
        if (string.IsNullOrWhiteSpace(publication.ChannelId))
            Add(ref errors, "channel.id.required", "channelId", "Reference the stable channel that contains this publication.");
        if (publication.Revision <= 0)
            Add(ref errors, "revision.invalid", "revision", "Revision must be a positive integer.");
        if (string.IsNullOrWhiteSpace(publication.Title))
            Add(ref errors, "title.required", "title", "Provide a non-empty publication title.");
        if (!IsVisibility(publication.Visibility))
            Add(ref errors, "visibility.invalid", "visibility", "Visibility must be public, unlisted, or private.");
        if (publication.PublishedAt.Offset != TimeSpan.Zero)
            Add(ref errors, "timestamp.not_utc", "publishedAt", "publishedAt must use UTC.");

        ValidateExperienceReference(publication.PrimaryExperience, ref errors);

        var seen = new HashSet<string>(StringComparer.Ordinal);
        for (int i = 0; i < publication.Resources.Count; i++)
        {
            TypedResourceReference? resource = publication.Resources[i];
            string field = "resources[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            if (resource is null)
            {
                Add(ref errors, "resource.required", field, "Remove the null resource reference or provide a valid reference.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(resource.ResourceId))
                Add(ref errors, "resource.id.required", field + ".resourceId", "Provide a stable resource identifier.");
            else if (!seen.Add(resource.ResourceId))
                Add(ref errors, "resource.id.duplicate", field + ".resourceId", "Resource identifiers must be unique within a publication snapshot.");
            if (string.IsNullOrWhiteSpace(resource.ResourceType))
                Add(ref errors, "resource.type.required", field + ".resourceType", "Provide an extensible resource type identifier.");
            if (string.IsNullOrWhiteSpace(resource.Role))
                Add(ref errors, "resource.role.required", field + ".role", "Provide the resource's role in this publication.");
            if (string.IsNullOrWhiteSpace(resource.MediaType))
                Add(ref errors, "resource.media_type.required", field + ".mediaType", "Provide the media type of the finalized resource.");
            if (!IsSha256(resource.Sha256))
                Add(ref errors, "resource.sha256.invalid", field + ".sha256", "SHA-256 must contain exactly 64 lowercase hexadecimal characters.");
            if (resource.LengthBytes < 0)
                Add(ref errors, "resource.length.invalid", field + ".lengthBytes", "Byte length cannot be negative.");
            if (resource.RightsDeclarationId is not null && string.IsNullOrWhiteSpace(resource.RightsDeclarationId))
                Add(ref errors, "resource.rights_reference.invalid", field + ".rightsDeclarationId", "Omit the rights reference or provide a non-empty identifier.");
            if (resource.ProvenanceRecordId is not null && string.IsNullOrWhiteSpace(resource.ProvenanceRecordId))
                Add(ref errors, "resource.provenance_reference.invalid", field + ".provenanceRecordId", "Omit the provenance reference or provide a non-empty identifier.");
        }

        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    private static void ValidateExperienceReference(ExperienceArtifactReference reference, ref List<PublicationValidationError>? errors)
    {
        if (reference.ExperienceId == 0)
            Add(ref errors, "experience.id.invalid", "primaryExperience.experienceId", "Experience ID must be greater than zero.");
        if (!IsVersionSegment(reference.Version))
            Add(ref errors, "experience.version.invalid", "primaryExperience.version", "Provide a non-empty version segment without path separators or control characters.");
        if (!IsSha256(reference.ContentHash))
            Add(ref errors, "experience.sha256.invalid", "primaryExperience.contentHash", "SHA-256 must contain exactly 64 hexadecimal characters.");
    }

    private static bool IsVersionSegment(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length > 128)
            return false;
        if (value == "." || value == "..")
            return false;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            bool allowed =
                (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') ||
                (c >= '0' && c <= '9') || c == '.' || c == '-' || c == '+' || c == '_';
            if (!allowed)
                return false;
        }
        return true;
    }

    private static bool IsVisibility(string value) =>
        string.Equals(value, "public", StringComparison.Ordinal) ||
        string.Equals(value, "unlisted", StringComparison.Ordinal) ||
        string.Equals(value, "private", StringComparison.Ordinal);

    private static bool IsSha256(string value)
    {
        if (value.Length != 64)
            return false;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f')))
                return false;
        }
        return true;
    }

    private static void Add(ref List<PublicationValidationError>? errors, string code, string field, string message)
    {
        errors ??= new List<PublicationValidationError>(4);
        errors.Add(new PublicationValidationError(code, field, message));
    }
}
