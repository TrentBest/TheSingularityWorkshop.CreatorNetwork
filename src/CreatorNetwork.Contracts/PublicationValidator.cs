using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>Allocation-conscious validation for untrusted channel manifests.</summary>
public static class PublicationValidator
{
    /// <summary>Validates a manifest and returns actionable errors. An empty list means valid.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(ChannelPublication? publication)
    {
        if (publication is null)
            return new[] { new PublicationValidationError("publication.required", "$", "A publication is required.") };

        List<PublicationValidationError>? errors = null;
        if (!string.Equals(publication.Schema, ChannelPublication.CurrentSchema, StringComparison.Ordinal))
            Add(ref errors, "schema.unsupported", "schema", $"Unsupported schema '{publication.Schema}'. Expected '{ChannelPublication.CurrentSchema}'.");
        if (string.IsNullOrWhiteSpace(publication.PublicationId))
            Add(ref errors, "publication.id.required", "publicationId", "Provide a stable publication identifier.");
        if (string.IsNullOrWhiteSpace(publication.Creator.Id))
            Add(ref errors, "creator.id.required", "creator.id", "Provide a stable creator identifier independent of a hosting account.");
        if (string.IsNullOrWhiteSpace(publication.Title))
            Add(ref errors, "title.required", "title", "Provide a non-empty channel title.");
        if (publication.Revision <= 0)
            Add(ref errors, "revision.invalid", "revision", "Revision must be a positive integer.");
        if (publication.UpdatedAt.Offset != TimeSpan.Zero)
            Add(ref errors, "timestamp.not_utc", "updatedAt", "Serialize updatedAt in UTC (zero offset).");
        if (!IsVisibility(publication.Visibility))
            Add(ref errors, "visibility.invalid", "visibility", "Visibility must be public, unlisted, or private.");

        IReadOnlyList<ContentReference> content = publication.Content;
        for (int i = 0; i < content.Count; i++)
        {
            ContentReference? item = content[i];
            string field = "content[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]";
            if (item is null)
            {
                Add(ref errors, "content.reference.required", field, "Remove the null content reference or provide a valid reference.");
                continue;
            }
            if (string.IsNullOrWhiteSpace(item.Id))
                Add(ref errors, "content.id.required", field + ".id", "Provide a stable content identifier.");
            if (string.IsNullOrWhiteSpace(item.MediaType))
                Add(ref errors, "content.media_type.required", field + ".mediaType", "Provide a media type.");
            if (item.LengthBytes < 0)
                Add(ref errors, "content.length.invalid", field + ".lengthBytes", "Byte length cannot be negative.");
            if (item.Sha256 is not null && !IsSha256(item.Sha256))
                Add(ref errors, "content.sha256.invalid", field + ".sha256", "SHA-256 must contain exactly 64 hexadecimal characters.");
        }

        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    private static bool IsVisibility(string value) =>
        string.Equals(value, "public", StringComparison.Ordinal) ||
        string.Equals(value, "unlisted", StringComparison.Ordinal) ||
        string.Equals(value, "private", StringComparison.Ordinal);

    private static bool IsSha256(string value)
    {
        if (value.Length != 64) return false;
        for (int i = 0; i < value.Length; i++)
        {
            char c = value[i];
            if (!((c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F')))
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
