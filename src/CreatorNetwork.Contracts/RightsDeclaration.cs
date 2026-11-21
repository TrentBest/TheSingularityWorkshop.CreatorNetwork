using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>Actions that a rights declaration may explicitly grant for one resource scope.</summary>
public enum RightsAction
{
    /// <summary>Store or host the resource.</summary>
    Host = 0,
    /// <summary>Display the resource.</summary>
    Display = 1,
    /// <summary>Perform or play the resource.</summary>
    Perform = 2,
    /// <summary>Transform the resource into a derivative representation.</summary>
    Transform = 3,
    /// <summary>Embed the resource in another work or experience.</summary>
    Embed = 4,
    /// <summary>Download a copy of the resource.</summary>
    Download = 5,
    /// <summary>Redistribute the resource to another party.</summary>
    Redistribute = 6,
    /// <summary>Create or distribute a remix or adaptation.</summary>
    Remix = 7,
    /// <summary>Grant downstream rights to another party.</summary>
    Sublicense = 8,
    /// <summary>Use the resource in a commercial context.</summary>
    CommercialUse = 9
}

/// <summary>Lifecycle state of a rights-holder declaration, not a legal verification result.</summary>
public enum RightsDeclarationStatus
{
    /// <summary>The rights-holder assertion is present but has not thereby been independently verified.</summary>
    Asserted = 0,
    /// <summary>The declaration or the authority behind it is disputed.</summary>
    Disputed = 1,
    /// <summary>The declaring party has withdrawn the declaration.</summary>
    Withdrawn = 2
}

/// <summary>
/// A scoped assertion of granted actions for one exact resource identity.
/// Absence of an action is not a grant. This declaration is not an authorization decision or legal conclusion.
/// </summary>
public sealed class RightsDeclaration
{
    /// <summary>Creates a scoped rights declaration. Validate untrusted instances before accepting them.</summary>
    public RightsDeclaration(
        string declarationId,
        string resourceId,
        string rightsHolderId,
        IReadOnlyList<RightsAction>? grantedActions,
        RightsDeclarationStatus status = RightsDeclarationStatus.Asserted,
        string? licenseReference = null,
        string? audienceScope = null,
        string? territory = null,
        DateTimeOffset? validFrom = null,
        DateTimeOffset? validUntil = null)
    {
        DeclarationId = declarationId ?? string.Empty;
        ResourceId = resourceId ?? string.Empty;
        RightsHolderId = rightsHolderId ?? string.Empty;
        GrantedActions = Snapshot(grantedActions);
        Status = status;
        LicenseReference = licenseReference;
        AudienceScope = audienceScope;
        Territory = territory;
        ValidFrom = validFrom;
        ValidUntil = validUntil;
    }

    private static IReadOnlyList<T> Snapshot<T>(IReadOnlyList<T>? source)
    {
        if (source is null || source.Count == 0)
            return Array.Empty<T>();

        var copy = new T[source.Count];
        for (int i = 0; i < copy.Length; i++)
            copy[i] = source[i];
        return Array.AsReadOnly(copy);
    }

    /// <summary>Stable identifier for this declaration/version.</summary>
    public string DeclarationId { get; }
    /// <summary>Exact resource identity to which the declaration applies; never a blanket channel grant.</summary>
    public string ResourceId { get; }
    /// <summary>Asserted rights-holder identity; verification and authority assessment are external.</summary>
    public string RightsHolderId { get; }
    /// <summary>Actions explicitly asserted as granted. Unlisted actions are not granted by this declaration.</summary>
    public IReadOnlyList<RightsAction> GrantedActions { get; }
    /// <summary>Current assertion state; disputed or withdrawn declarations must not be treated as active grants.</summary>
    public RightsDeclarationStatus Status { get; }
    /// <summary>Optional stable reference to a license or agreement outside this contract.</summary>
    public string? LicenseReference { get; }
    /// <summary>Optional audience scope identifier defined by the integrating host/domain.</summary>
    public string? AudienceScope { get; }
    /// <summary>Optional territory expression defined by the referenced agreement.</summary>
    public string? Territory { get; }
    /// <summary>Optional inclusive start instant, expressed in UTC.</summary>
    public DateTimeOffset? ValidFrom { get; }
    /// <summary>Optional exclusive end instant, expressed in UTC.</summary>
    public DateTimeOffset? ValidUntil { get; }
}

/// <summary>Structural validation for a scoped rights declaration.</summary>
public static class RightsDeclarationValidator
{
    /// <summary>Validates declaration structure; does not verify rights-holder authority or decide permission.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(RightsDeclaration? declaration)
    {
        if (declaration is null)
            return new[] { new PublicationValidationError("rights.declaration.required", "rights", "A rights declaration is required.") };

        List<PublicationValidationError>? errors = null;
        if (string.IsNullOrWhiteSpace(declaration.DeclarationId))
            Add(ref errors, "rights.id.required", "declarationId", "Provide a stable declaration identifier.");
        if (string.IsNullOrWhiteSpace(declaration.ResourceId))
            Add(ref errors, "rights.resource.required", "resourceId", "Scope the declaration to one exact resource identity.");
        if (string.IsNullOrWhiteSpace(declaration.RightsHolderId))
            Add(ref errors, "rights.holder.required", "rightsHolderId", "Identify the asserted rights holder.");
        if (!Enum.IsDefined(typeof(RightsDeclarationStatus), declaration.Status))
            Add(ref errors, "rights.status.invalid", "status", "The rights declaration status is not supported.");
        if (declaration.GrantedActions.Count == 0)
            Add(ref errors, "rights.actions.required", "grantedActions", "List at least one action explicitly granted by this declaration.");

        var seen = new HashSet<RightsAction>();
        for (int i = 0; i < declaration.GrantedActions.Count; i++)
        {
            RightsAction action = declaration.GrantedActions[i];
            if (!Enum.IsDefined(typeof(RightsAction), action))
                Add(ref errors, "rights.action.invalid", "grantedActions[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]", "The rights action is not supported.");
            else if (!seen.Add(action))
                Add(ref errors, "rights.action.duplicate", "grantedActions[" + i.ToString(System.Globalization.CultureInfo.InvariantCulture) + "]", "Rights actions must be unique.");
        }

        if (declaration.ValidFrom.HasValue && declaration.ValidFrom.Value.Offset != TimeSpan.Zero)
            Add(ref errors, "rights.valid_from.not_utc", "validFrom", "validFrom must use UTC.");
        if (declaration.ValidUntil.HasValue && declaration.ValidUntil.Value.Offset != TimeSpan.Zero)
            Add(ref errors, "rights.valid_until.not_utc", "validUntil", "validUntil must use UTC.");
        if (declaration.ValidFrom.HasValue && declaration.ValidUntil.HasValue &&
            declaration.ValidUntil.Value <= declaration.ValidFrom.Value)
            Add(ref errors, "rights.validity.invalid", "validUntil", "validUntil must be later than validFrom.");

        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    private static void Add(ref List<PublicationValidationError>? errors, string code, string field, string message)
    {
        errors ??= new List<PublicationValidationError>(4);
        errors.Add(new PublicationValidationError(code, field, message));
    }
}
