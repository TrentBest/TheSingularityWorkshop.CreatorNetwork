![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork publication contracts

CreatorNetwork is the creator-facing Content Creator platform in The Singularity Workshop. Its portable contracts let creators publish work assembled across tools, devices, services, media, data, simulations, MicroBundles, and Experiences without making one editor, runtime, storage provider, discovery service, or aggregator the owner of that work.

The contracts are deliberately smaller than the platform. StoryBuilder, Experiences, UserIO/capture adapters, MicroBundleDomain, MicroBundleRepository, FSM_COS, media providers, and host applications retain their own domain responsibilities. CreatorNetwork connects their finalized outputs through portable channel and publication identity, provenance, rights declarations, and distribution policy.

## 1. Two lifecycles: channel and publication

A **channel** is the stable creator-owned collection. It may exist while empty, and it may contain multiple publications. Channel title, description, and declared visibility can change without rewriting an already released publication.

A **publication** is one immutable, versioned snapshot inside a channel. Updating the published work creates a new revision. A channel metadata edit does not silently mutate a publication snapshot, and an earlier revision remains addressable for caches, exports, provenance, and audit.

## 2. Contract generations

### v1: retained compatibility

`ChannelPublication` with schema `creator-network.channel/1` is the existing first-generation wire DTO. It combines a publication identifier/revision with fields that were originally described as channel metadata. It remains supported as-is for compatibility.

Do not silently reinterpret its fields, rename its identifiers, rewrite its revision history, or change its serialized shape. The existing local Creator Node currently uses this v1 shape.

### v2: separated portable records

The additive v2 model introduces:

- `CreatorChannel` — stable `ChannelId`, creator projection, channel metadata, declared visibility, and creation/update timestamps. It does not contain a publication list; an empty channel is valid.
- `PublicationSnapshot` — stable `PublicationId`, `ChannelId`, positive revision, publication-time metadata/visibility, a primary Experience artifact reference, and ordered supporting-resource references.
- `ExperienceArtifactReference` — the Experiences domain's numeric identity, explicit artifact version, and SHA-256 digest. This matches the existing immutable Experience address tuple without importing MicroBundleRepository storage, transport, or catalog behavior.
- `TypedResourceReference` — a stable resource ID, extensible type identifier, publication role, media type, SHA-256 digest, optional byte length, optional location hint, and optional provenance/rights declaration references.

The v2 types are provider-neutral contract models. Their structural validators reject malformed identities, versions, digests, timestamps, and duplicate resource identities.

## 3. Keep executable artifacts and supporting resources distinct

The primary Experience reference identifies a finalized Experience artifact. The artifact bytes and publication-pointer lifecycle remain owned by the Experiences/MicroBundleRepository path. CreatorNetwork does not copy those bytes or redefine MicroBundle identity.

A `TypedResourceReference` is for non-executable supporting content such as an image, mesh, document, dataset, transcript, or finalized media resource. It is not a MicroBundle, repository, byte store, or delivery endpoint. A media provider owns media ingest and delivery; a destination provider independently decides whether it can import and serve a resource.

A resource's `SourceUri`, when present, is only an untrusted location hint. It is not identity, integrity evidence, permission, or an instruction to fetch. Importers must apply explicit origin-fetch policy and SSRF protections.

## 4. Rights, provenance, and access remain separate

A SHA-256 digest identifies exact bytes; it does not establish authorship, ownership, license, or authorization. Provenance and rights declaration IDs are references to separate claims and evidence structures. Their presence does not prove that the claims are true.

Visibility is descriptive metadata, not access control. A trusted host/provider must authenticate the actor, assess authority, evaluate the applicable sharing and rights policy, and enforce decisions for metadata, bytes, ranges, caches, exports, and imports. A destination independently authorizes import and future serving.

Consumer capability compatibility belongs to the Experience manifest/resolver and host. A client that cannot satisfy a required capability must receive an explicit incompatibility result; CreatorNetwork must not silently flatten or discard required content.

## 5. Current implementation status

<badge color="success">Implemented and CI-verified</badge>

- v1 `ChannelPublication` remains unchanged.
- v2 `CreatorChannel`, `PublicationSnapshot`, `ExperienceArtifactReference`, and `TypedResourceReference` exist as additive contract models.
- Structural validators cover required fields, UTC timestamps, supported visibility values, positive publication revisions, path-safe Experience version segments, SHA-256 shape, and duplicate resource IDs.
- Tests prove an empty channel is valid, multiple revisions can share a stable channel/publication identity, resource collections are defensively snapshotted, and the v1 contract remains valid.
- The Experience reference carries no provider URL, so its identity tuple is independent of a provider location.

Verification on commit [`c70f6c5`](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/commit/c70f6c566d5a3e23ae56917c4329ac7a4f7a16fe):
- [CreatorNetwork CI](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/actions/runs/45754538338) — passed, including contract/authoring/node tests, Profiles integration, museum consumer execution, and benchmark build.
- [Sith Policy](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/actions/runs/45754538325) — passed.

### Not yet implemented or proven

These contracts are not yet wired into the Creator Node, persisted as channel/publication records, or exposed through new endpoints. No provider-backed CreatorNetwork publisher/consumer, two-provider export/import, capability-negotiation workflow, publication-history store, or rights/sharing enforcement for v2 is claimed. The current museum consumer remains an in-memory contract demonstration.

The next vertical slice must connect a creator-side publication workflow to the existing immutable Experience and MicroBundle providers, then retrieve it from a separate consumer. The current MicroBundleRepository development host has Experience REST routes in Azure mode, but its Local mode still lacks an Experience catalog/repository; that upstream gap is tracked in [MicroBundleRepository issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22). Do not work around that gap by duplicating artifact storage inside CreatorNetwork.

## 6. Compatibility and migration policy

- Existing v1 readers and writers remain supported until an explicit migration plan is approved.
- v2 is additive; no automatic v1-to-v2 conversion is performed.
- A future migration tool must preserve creator/channel/publication identifiers and revision history, and must report any mapping that cannot be established from source evidence.
- Providers may store v1 and v2 records differently, but portable identity and schema semantics must remain stable across provider migration.
- A provider URL, local filesystem path, account ID, or discovery index must never become canonical publication identity.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
