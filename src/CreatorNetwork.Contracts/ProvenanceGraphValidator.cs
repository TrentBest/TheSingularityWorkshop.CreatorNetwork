using System;
using System.Collections.Generic;
using System.Globalization;

namespace CreatorNetwork.Contracts;

/// <summary>Structural validation for a supplied set of provenance records.</summary>
/// <remarks>
/// Validation detects malformed records, duplicate outputs, and cycles among records present in the
/// supplied set. It deliberately does not claim that the graph is complete, that external references
/// exist, that assertions are true, or that any operation is licensed.
/// </remarks>
public static class ProvenanceGraphValidator
{
    /// <summary>Validates records as one graph snapshot and returns actionable errors.</summary>
    public static IReadOnlyList<PublicationValidationError> Validate(IReadOnlyList<ProvenanceRecord?>? records)
    {
        if (records is null)
            return new[] { new PublicationValidationError("provenance.records.required", "provenance", "A provenance record collection is required.") };

        List<PublicationValidationError>? errors = null;
        var recordIds = new HashSet<string>(StringComparer.Ordinal);
        var outputOwners = new Dictionary<string, int>(StringComparer.Ordinal);

        for (int i = 0; i < records.Count; i++)
        {
            ProvenanceRecord? record = records[i];
            string field = "provenance[" + i.ToString(CultureInfo.InvariantCulture) + "]";
            if (record is null)
            {
                Add(ref errors, "provenance.record.required", field, "A provenance record cannot be null.");
                continue;
            }

            if (string.IsNullOrWhiteSpace(record.RecordId))
                Add(ref errors, "provenance.record_id.required", field + ".recordId", "Provide a stable provenance record identifier.");
            else if (!recordIds.Add(record.RecordId))
                Add(ref errors, "provenance.record_id.duplicate", field + ".recordId", "Record identifiers must be unique within the graph snapshot.");

            if (string.IsNullOrWhiteSpace(record.OutputContentId))
                Add(ref errors, "provenance.output.required", field + ".outputContentId", "Identify the exact output content artifact.");
            else if (outputOwners.ContainsKey(record.OutputContentId))
                Add(ref errors, "provenance.output.duplicate", field + ".outputContentId", "Each output content artifact must have one creating record in this graph snapshot.");
            else
                outputOwners.Add(record.OutputContentId, i);

            if (string.IsNullOrWhiteSpace(record.ActorId))
                Add(ref errors, "provenance.actor.required", field + ".actorId", "Identify the actor who performed the operation.");
            if (!Enum.IsDefined(typeof(ProvenanceOriginatorStatus), record.OriginatorStatus))
                Add(ref errors, "provenance.originator_status.invalid", field + ".originatorStatus", "The originator status is not supported.");
            if (record.OriginatorStatus == ProvenanceOriginatorStatus.Claimed && string.IsNullOrWhiteSpace(record.OriginatorId))
                Add(ref errors, "provenance.originator.required", field + ".originatorId", "An originator identity is required when the originator status is claimed.");
            if (record.OriginatorStatus == ProvenanceOriginatorStatus.Unknown && !string.IsNullOrWhiteSpace(record.OriginatorId))
                Add(ref errors, "provenance.originator.status_conflict", field + ".originatorId", "An unknown originator status cannot include an originator identity.");
            if (string.IsNullOrWhiteSpace(record.Operation))
                Add(ref errors, "provenance.operation.required", field + ".operation", "Describe the operation performed.");
            if (record.OccurredAt.Offset != TimeSpan.Zero)
                Add(ref errors, "provenance.timestamp.not_utc", field + ".occurredAt", "Serialize the asserted operation time in UTC.");

            for (int inputIndex = 0; inputIndex < record.Inputs.Count; inputIndex++)
            {
                ProvenanceReference? input = record.Inputs[inputIndex];
                string inputField = field + ".inputs[" + inputIndex.ToString(CultureInfo.InvariantCulture) + "]";
                if (input is null)
                {
                    Add(ref errors, "provenance.input.required", inputField, "A provenance input cannot be null.");
                    continue;
                }
                if (string.IsNullOrWhiteSpace(input.Id))
                    Add(ref errors, "provenance.input_id.required", inputField + ".id", "Provide a stable identifier for the upstream item.");
                if (!Enum.IsDefined(typeof(ProvenanceReferenceKind), input.Kind))
                    Add(ref errors, "provenance.input_kind.invalid", inputField + ".kind", "The provenance reference kind is not supported.");
                if (input.Kind == ProvenanceReferenceKind.ContentArtifact &&
                    string.Equals(input.Id, record.OutputContentId, StringComparison.Ordinal))
                    Add(ref errors, "provenance.self_reference", inputField, "An output cannot be its own input.");
            }

            for (int changeIndex = 0; changeIndex < record.Changes.Count; changeIndex++)
            {
                if (string.IsNullOrWhiteSpace(record.Changes[changeIndex]))
                    Add(ref errors, "provenance.change.empty", field + ".changes[" + changeIndex.ToString(CultureInfo.InvariantCulture) + "]", "Change descriptions cannot be empty.");
            }
        }

        DetectCycles(records, outputOwners, ref errors);
        return errors is null ? Array.Empty<PublicationValidationError>() : errors;
    }

    private static void DetectCycles(
        IReadOnlyList<ProvenanceRecord?> records,
        Dictionary<string, int> outputOwners,
        ref List<PublicationValidationError>? errors)
    {
        var state = new byte[records.Count];
        for (int i = 0; i < records.Count; i++)
            Visit(i, records, outputOwners, state, ref errors);
    }

    private static void Visit(
        int index,
        IReadOnlyList<ProvenanceRecord?> records,
        Dictionary<string, int> outputOwners,
        byte[] state,
        ref List<PublicationValidationError>? errors)
    {
        if (state[index] == 2 || state[index] == 1)
            return;

        state[index] = 1;
        ProvenanceRecord? record = records[index];
        if (record is not null)
        {
            for (int i = 0; i < record.Inputs.Count; i++)
            {
                ProvenanceReference? input = record.Inputs[i];
                if (input is null || input.Kind != ProvenanceReferenceKind.ContentArtifact ||
                    !outputOwners.TryGetValue(input.Id, out int parentIndex))
                    continue;

                if (state[parentIndex] == 1)
                {
                    Add(ref errors, "provenance.cycle", "provenance[" + index.ToString(CultureInfo.InvariantCulture) + "].inputs[" + i.ToString(CultureInfo.InvariantCulture) + "]", "The supplied provenance records contain a lineage cycle.");
                    continue;
                }
                Visit(parentIndex, records, outputOwners, state, ref errors);
            }
        }
        state[index] = 2;
    }

    private static void Add(ref List<PublicationValidationError>? errors, string code, string field, string message)
    {
        errors ??= new List<PublicationValidationError>(4);
        errors.Add(new PublicationValidationError(code, field, message));
    }
}
