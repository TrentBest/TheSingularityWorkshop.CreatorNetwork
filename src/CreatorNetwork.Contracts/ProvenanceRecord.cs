using System;
using System.Collections.Generic;

namespace CreatorNetwork.Contracts;

/// <summary>
/// An append-only assertion describing how one digital artifact was created or modified.
/// The record captures declared lineage; it is not independent proof of authorship or license.
/// </summary>
public sealed class ProvenanceRecord
{
    /// <summary>Creates a provenance record. Validate untrusted instances before accepting them.</summary>
    public ProvenanceRecord(
        string recordId,
        string outputContentId,
        string actorId,
        string? originatorId,
        string operation,
        DateTimeOffset occurredAt,
        IReadOnlyList<ProvenanceReference>? inputs = null,
        IReadOnlyList<string>? changes = null,
        string? toolName = null,
        string? toolVersion = null)
    {
        RecordId = recordId ?? string.Empty;
        OutputContentId = outputContentId ?? string.Empty;
        ActorId = actorId ?? string.Empty;
        OriginatorId = originatorId;
        Operation = operation ?? string.Empty;
        OccurredAt = occurredAt;
        Inputs = Snapshot(inputs);
        Changes = Snapshot(changes);
        ToolName = toolName;
        ToolVersion = toolVersion;
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

    /// <summary>Stable identifier for this provenance assertion.</summary>
    public string RecordId { get; }
    /// <summary>Stable identifier of the exact output content artifact/version.</summary>
    public string OutputContentId { get; }
    /// <summary>Actor who performed the recorded operation; an asserted identity until verified by a host.</summary>
    public string ActorId { get; }
    /// <summary>Optional asserted originator of the represented work; not a verified rights-holder identity.</summary>
    public string? OriginatorId { get; }
    /// <summary>Controlled or documented operation label, such as capture, scan, edit, composite, or conversion.</summary>
    public string Operation { get; }
    /// <summary>UTC timestamp at which the operation is asserted to have occurred.</summary>
    public DateTimeOffset OccurredAt { get; }
    /// <summary>Typed upstream references, including real-world subjects, logical works, and exact input artifacts.</summary>
    public IReadOnlyList<ProvenanceReference> Inputs { get; }
    /// <summary>Human-readable change description; exact machine-verifiable diffs belong to referenced evidence/artifacts.</summary>
    public IReadOnlyList<string> Changes { get; }
    /// <summary>Optional tool or process name used for the operation.</summary>
    public string? ToolName { get; }
    /// <summary>Optional exact tool/process version.</summary>
    public string? ToolVersion { get; }
}
