using System;
using System.Collections.Generic;
using CreatorNetwork.Contracts;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class ProvenanceGraphValidatorTests
{
    [Fact]
    public void ValidSourceAndDerivativeLineageHasNoErrors()
    {
        var source = new ProvenanceRecord(
            "record:scan",
            "content:scan-v1",
            "creator:museum",
            "originator:museum",
            "capture",
            DateTimeOffset.UtcNow,
            new[] { new ProvenanceReference(ProvenanceReferenceKind.RealWorldReferent, "object:statue") });

        var derivative = new ProvenanceRecord(
            "record:low-poly",
            "content:low-poly-v1",
            "creator:museum",
            "originator:museum",
            "authorized-representation",
            DateTimeOffset.UtcNow,
            new[]
            {
                new ProvenanceReference(ProvenanceReferenceKind.DigitalWork, "work:statue"),
                new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:scan-v1")
            },
            new[] { "Reduced polygon count using the museum-approved representation workflow." },
            "Museum representation pipeline",
            "1.0");

        Assert.Empty(ProvenanceGraphValidator.Validate(new[] { source, derivative }));
    }

    [Fact]
    public void DuplicateOutputAndCycleAreRejected()
    {
        var first = new ProvenanceRecord(
            "record:first", "content:first", "actor:a", null, "edit", DateTimeOffset.UtcNow,
            new[] { new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:second") });
        var second = new ProvenanceRecord(
            "record:second", "content:second", "actor:b", null, "edit", DateTimeOffset.UtcNow,
            new[] { new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:first") });
        var duplicate = new ProvenanceRecord(
            "record:duplicate", "content:first", "actor:c", null, "copy", DateTimeOffset.UtcNow);

        IReadOnlyList<PublicationValidationError> errors =
            ProvenanceGraphValidator.Validate(new[] { first, second, duplicate });

        Assert.Contains(errors, x => x.Code == "provenance.output.duplicate");
        Assert.Contains(errors, x => x.Code == "provenance.cycle");
    }

    [Fact]
    public void InvalidActorAndNonUtcTimestampAreRejected()
    {
        var record = new ProvenanceRecord(
            "record:bad", "content:bad", "", null, "", 
            new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.FromHours(2)));

        IReadOnlyList<PublicationValidationError> errors =
            ProvenanceGraphValidator.Validate(new[] { record });

        Assert.Contains(errors, x => x.Code == "provenance.actor.required");
        Assert.Contains(errors, x => x.Code == "provenance.operation.required");
        Assert.Contains(errors, x => x.Code == "provenance.timestamp.not_utc");
    }

    [Fact]
    public void ProvenanceInputsAreSnapshotted()
    {
        var inputs = new List<ProvenanceReference>
        {
            new(ProvenanceReferenceKind.RealWorldReferent, "object:one")
        };
        var record = new ProvenanceRecord(
            "record:snapshot", "content:snapshot", "actor:one", null, "capture",
            DateTimeOffset.UtcNow, inputs);

        inputs.Add(new ProvenanceReference(ProvenanceReferenceKind.ContentArtifact, "content:later"));

        Assert.Single(record.Inputs);
    }

    [Fact]
    public void NullCollectionIsInvalid()
    {
        var error = Assert.Single(ProvenanceGraphValidator.Validate(null));
        Assert.Equal("provenance.records.required", error.Code);
    }
}
