![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# 4. A vertical slice: from record to consumer

A portable publication matters when a consumer can use it independently of the authoring tool and storage provider. Use the repository's [Getting Started guide](../GETTING_STARTED.md) for the current runnable path; this chapter explains what the proof does and does not establish.

## Conceptual flow

```text
Creator / author
   |
   v
Authoring client or supplied publication record
   |
   v
ChannelPublication + ContentReference
   |
   v
PublicationValidator
   |
   v
Creator Node / publication store
   |
   v
Independent client resolves the record and content location
   |
   v
Client chooses presentation
```

This is a conceptual map of the current channel/media slice, not proof that every step is integrated end-to-end in a production deployment. Confirm exact commands and supported paths in the operational guide.

## How to assess evidence

- **Contract code** shows which fields and validation rules exist.
- **Unit tests** show the inputs and outcomes explicitly exercised.
- **Node/client tests** can show particular storage and transport behavior, within the tested setup.
- **CI** shows a workflow ran at a particular commit; it does not prove production hosting, general interoperability, or the absence of untested failure modes.
- **Benchmarks** characterize measured scenarios and are not a correctness or security proof.

The contracts tests include [PublicationValidatorTests.cs](../../tests/CreatorNetwork.Contracts.Tests/PublicationValidatorTests.cs), [FilePublicationStoreTests.cs](../../tests/CreatorNetwork.Contracts.Tests/FilePublicationStoreTests.cs), [FileMediaStoreTests.cs](../../tests/CreatorNetwork.Contracts.Tests/FileMediaStoreTests.cs), and [CreatorNodeClientTests.cs](../../tests/CreatorNetwork.Contracts.Tests/CreatorNodeClientTests.cs). Read each test for its exact coverage before making broader claims.

## Current gaps to keep visible

The current implementation is a local proving slice, not a complete general artifact/Experience publication system. Do not mark these complete without source and test evidence:

- General publication types for arbitrary MicroBundles, Experiences, and typed resources.
- A complete export/import and migration path that preserves identity and revision semantics.
- Independent publisher and consumer programs proving provider replacement.
- Production-grade authentication/authorization, remote-fetch protections, abuse controls, and operational availability.
- Verified rights/provenance workflows and audience portability.

## Acceptance question

Can a creator publish a stable record, have a separate consumer resolve it, and move the publication to another provider without rewriting creator identity or the publication itself? Each step should name the implementation, the test that exercises it, and the failure behavior when unavailable or unauthorized.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
