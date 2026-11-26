using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>Structural validation for portable channel metadata.</summary>
public static class CreatorChannelValidator
{
    /// <summary>Validates channel structure; does not authenticate the creator or enforce visibility.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(CreatorChannel? channel)
    {
        if (channel is null)
            return new[] { new PublicationValidationError("channel.required", "$", "A channel is required.") };

        List<PublicationValidationError>? errors = null;
        if (!string.Equals(channel.Schema, CreatorChannel.CurrentSchema, StringComparison.Ordinal))
            Add(ref errors, "schema.unsupported", "schema", $"Unsupported schema '{channel.Schema}'. Expected '{CreatorChannel.CurrentSchema}'.");
        if (string.IsNullOrWhiteSpace(channel.ChannelId))
            Add(ref errors, "channel.id.required", "channelId", "Provide a stable portable channel identifier.");
        if (string.IsNullOrWhiteSpace(channel.Creator.Id))
            Add(ref errors, "creator.id.required", "creator.id", "Provide a stable creator identifier.");
        if (string.IsNullOrWhiteSpace(channel.Title))
            Add(ref errors, "channel.title.required", "title", "Provide a non-empty channel title.");
        if (!IsVisibility(channel.Visibility))
            Add(ref errors, "visibility.invalid", "visibility", "Visibility must be public, unlisted, or private.");
        if (channel.CreatedAt.Offset != TimeSpan.Zero)
            Add(ref errors, "timestamp.created_at.not_utc", "createdAt", "createdAt must use UTC.");
        if (channel.UpdatedAt.Offset != TimeSpan.Zero)
            Add(ref errors, "timestamp.updated_at.not_utc", "updatedAt", "updatedAt must use UTC.");
        if (channel.UpdatedAt < channel.CreatedAt)
            Add(ref errors, "timestamp.order.invalid", "updatedAt", "updatedAt cannot precede createdAt.");

        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    private static bool IsVisibility(string value) =>
        string.Equals(value, "public", StringComparison.Ordinal) ||
        string.Equals(value, "unlisted", StringComparison.Ordinal) ||
        string.Equals(value, "private", StringComparison.Ordinal);

    private static void Add(ref List<PublicationValidationError>? errors, string code, string field, string message)
    {
        errors ??= new List<PublicationValidationError>(4);
        errors.Add(new PublicationValidationError(code, field, message));
    }
}
