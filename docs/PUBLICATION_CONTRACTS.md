![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork publication contract

CreatorNetwork is the creator-facing Content Creator platform in The Singularity Workshop. This release has one portable publication contract: `creator-network.channel/1`. The same contract is used by the contracts library, Creator Node, authoring client, samples, and integrations. There are no parallel schema generations in this release.

## One record, one schema

The existing `ChannelPublication` DTO is the authoritative wire contract. It carries the publication identity and revision, creator projection, title and description, declared visibility, update timestamp, content references, and attribution together.

Do not introduce a second channel DTO or a separate publication-snapshot DTO alongside it. Do not reinterpret, rename, or silently change serialized fields. If the existing shape needs improvement, reconcile the change across every consumer and producer as one coordinated interface change before shipping the package.

## Ownership boundaries

- StoryBuilder owns canonical story records and narrative authoring.
- Experiences owns Experience composition and finalization.
- UserIO and host-owned adapters own interaction and source acquisition.
- MicroBundleDomain owns MicroBundle semantics.
- MicroBundleRepository owns immutable artifact storage and retrieval.
- Media providers own media ingest, storage, and delivery.
- Profiles supplies identity and disclosure semantics.
- Trusted hosts/providers authenticate actors and enforce authorization.
- CreatorNetwork coordinates publication, portable references, provenance/rights declarations, discovery/addressing, and migration.

CreatorNetwork must not duplicate artifact or media storage to work around an upstream provider gap.

## Trust and resource rules

A digest identifies exact bytes; it does not prove authorship, ownership, license, or permission. Provenance and rights declarations are claims that require evaluation. Declared visibility is metadata, not access control. Trusted hosts/providers enforce policy for metadata, bytes, ranges, caches, exports, and imports.

A source URI is an untrusted location hint, not identity, integrity evidence, permission, or an instruction to fetch. Importers must apply explicit origin-fetch policy and SSRF protections.

## Current implementation status

The local Creator Node accepts the existing `creator-network.channel/1` shape. The authoring client and current integrations must continue to use this same contract. The museum consumer is an in-memory contract demonstration; it is not proof of independent provider delivery.

The local Creator Node remains loopback-only and unauthenticated. Do not expose it to a network or use it for real private content. Production identity, authorization, rights evaluation, provider-backed publication/consumption, export/import, and independent-host interoperability remain unproven.

## Release rule

A package release contains one reconciled interface generation. Any contract change must be propagated to all known producers, consumers, tests, examples, documentation, and dependent repositories in the same coordinated change. Do not ship parallel interface generations or leave pre-release consumers to guess which contract to implement.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
