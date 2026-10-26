namespace CreatorNetwork.Contracts;

/// <summary>One actionable publication validation failure.</summary>
public sealed class PublicationValidationError
{
    /// <summary>Creates a validation error.</summary>
    public PublicationValidationError(string code, string field, string message)
    {
        Code = code;
        Field = field;
        Message = message;
    }

    /// <summary>Stable machine-readable error code.</summary>
    public string Code { get; }
    /// <summary>Field or path associated with the error.</summary>
    public string Field { get; }
    /// <summary>Human-readable remediation guidance.</summary>
    public string Message { get; }

    /// <inheritdoc />
    public override string ToString() => $"{Code} ({Field}): {Message}";
}
