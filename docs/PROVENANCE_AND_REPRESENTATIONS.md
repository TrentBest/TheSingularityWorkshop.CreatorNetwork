![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Provenance and rights-aware representations

This document defines the first additive provenance contract in CreatorNetwork. It records asserted lineage without pretending that metadata proves ownership, authenticity, or permission. It also defines the museum representation workflow that the contract is intended to support.

## 1. The three identities that must not collapse

A reusable digital item can refer to three different things:

1. **Real-world referent** — the physical sculpture, building, person, place, event, or other subject.
2. **Logical work** — the creative or intellectual work represented by one or more digital files.
3. **Digital artifact** — an exact file or version, identified independently of its download URL.

A scan of a sculpture is not the sculpture. A compressed mesh is not the original scan. A painting photograph is not the painting. A derivative may preserve the same logical work while producing different bytes and a different artifact identity.

The initial `ProvenanceReferenceKind` values distinguish real-world referents, logical digital works, exact content artifacts, and external evidence. Identifiers are opaque and stable; URLs are not identity.

## 2. Who did what, to which exact output

An additive `ProvenanceRecord` describes one operation and one exact output artifact:

- `RecordId`: stable identifier for the assertion.
- `OutputContentId`: exact output artifact/version.
- `ActorId`: actor asserted to have performed the operation.
- `OriginatorId`: asserted originator identifier when one is claimed.
- `OriginatorStatus`: explicit `Claimed`, `Unknown`, or `Disputed` state. A claimed identity is still an assertion, not a verification result.
- `Operation`: capture, scan, edit, composite, conversion, approved representation, or another documented operation.
- `OccurredAt`: asserted UTC time.
- `Inputs`: typed upstream references, including exact input artifacts.
- `Changes`: human-readable account of what changed.
- `ToolName` and `ToolVersion`: optional reproducibility context.

Each derivative receives a new output artifact identity and a new record. A composite lists every contributing input. The record is append-only by design: corrections should be represented by a superseding assertion or a new graph snapshot, not by silently rewriting the historical account.

The graph validator checks required fields, explicit originator-claim consistency, duplicate record/output identifiers, self-reference, and cycles among records supplied together. A content-artifact reference not represented by a record in the supplied snapshot is treated as an external boundary, not silently rejected. Therefore, structural validation does **not** prove that ancestry is complete. Importers and publishers must state the graph coverage they have actually verified.

## 3. Provenance is not proof or permission

Keep these separate:

| Concern | What it establishes | What it does not establish |
|---|---|---|
| Provenance assertion | A declared chain of inputs, actors, operations, and outputs | Truth of the assertion |
| Integrity digest | Exact byte equality against a known digest | Origin, authorship, or license |
| Identity verification | Control of an identity under a host's verification method | Ownership of every work attributed to that identity |
| License or permission evidence | A claim or evidence of granted rights within a stated scope | Rights outside that scope |
| Authorization decision | A host's decision for one actor, action, resource, and context | A universal right to reuse elsewhere |

A creator may assert that they originated a work. A museum may assert that it controls a collection or a particular representation. A claimant may submit evidence. Those statements remain distinguishable from independently verified facts and from the host's authorization decision.

The initial contract deliberately does not encode a universal legal license language or calculate whether a use is lawful. The additive `RightsDeclaration` identifies one exact resource, an asserted rights holder, an explicit set of granted actions, assertion status, optional license reference, audience/territory scope, and validity window. Actions distinguish hosting, display, performance, transformation, embedding, download, redistribution, remix, sublicensing, caching, replication, and commercial use. An unlisted action is not granted. A grant for one action must not imply another.

The status is `Asserted`, `Disputed`, or `Withdrawn`; a disputed or withdrawn declaration must not be treated as an active grant. A declaration is still a claim, not proof that the declaring identity controls the work. A trusted host verifies the claimant's authority and makes the contextual authorization decision.

## 4. Museum-controlled subresources and approved alternatives

A museum or collection owner can be both a publisher and the rights authority for selected resources. A single Experience may contain separately governed resources:

- the museum building or room layout;
- signage, labels, and plaques;
- a sculpture or painting;
- a scan or photograph of that work;
- an approved simplified or low-poly representation;
- an audio guide or transcript.

Permission to reuse the building does not automatically grant permission to reuse the art displayed inside it. A publication must not flatten a venue into one blanket rights decision.

When an original representation is restricted, the source owner may publish an approved substitute. The consumer requests a semantic role (for example, `gallery.centerpiece`) and resolves an eligible representation through the source owner's declared alternatives. The resolver evaluates the selected artifact's own rights, provenance, compatibility, and version. It must not independently transform a restricted source to evade the restriction.

A `RepresentationOffer` ties the publisher assertion to the subject, exact source artifact, distinct alternative artifact, semantic role, alternative provenance record, and a rights declaration scoped to the alternative. It is a source-published mapping, not proof that the publisher has authority or that the consumer may use it.

A depolygon or simplification workflow is a transformation, not a rights bypass. It may run only when the relevant transformation and resulting use are authorized. The output receives a new content identity and provenance record identifying the exact source, actor, tool/version, and changes.

## 5. Swappable collections without rebuilding an Experience

An Experience should depend on stable semantic slots and declared compatibility, not hard-coded museum file URLs. For example, a gallery can request:

- `gallery.centerpiece` — one sculpture representation;
- `gallery.plaque` — optional explanatory text or plaque;
- `gallery.audio-guide` — optional narration;
- `gallery.environment` — a compatible room or scene resource.

Museum A and Museum B may publish different representations for the same slot. A consumer can replace the source collection while retaining the Experience's composition and role mapping, provided the replacement satisfies the slot's compatibility requirements and is independently authorized.

Resolution is not a one-time bypass. Re-evaluate rights and compatibility when the source version changes, a license expires, a representation is withdrawn, a claim is substantiated, or access is revoked. Cache keys and cached metadata must preserve the source artifact/version and authorization context so stale approvals do not silently survive a policy change.

## 6. Composite lineage and commercial use

A composite must retain references to all contributing inputs, not just the last editor. Derivative lineage should be transitive: a consumer can traverse from the current artifact through each transformation to the declared upstream works and source artifacts.

An ancestry relationship does not automatically create a royalty obligation. Compensation requires an explicit agreement defining eligible uses, parties, attribution, rates or formulas, reporting, thresholds, territory, duration, audit, disputes, and settlement. CreatorNetwork can carry references to such declarations or agreements when the future contract is defined. It should not become the accounting ledger or payment processor.

The eventual Economy/accounting/payment domain should own calculation and settlement. A commercial Experience should be able to report which versioned contributions were actually included and under which agreement, without treating every ancestor as automatically entitled to payment.

## 7. Creator-owned venues and tools

A creator's studio, gallery, workshop, recording room, sculpture space, painting studio, or physics-enabled voxel workshop is authored content. It can be published as an Experience or MicroBundle composition through the existing Workshop authoring and finalization path.

Ownership remains separated:

- Experiences authors and finalizes the venue and composition.
- MicroBundleDomain defines capability meaning; MicroBundleRepository stores and retrieves immutable MicroBundle artifacts.
- CreatorNetwork publishes references, provenance, rights declarations, and discovery metadata.
- FSM_COS composes verified capabilities; the trusted host authorizes and executes.
- Physics owns simulation and physical interaction.
- Renderer, Visualization, and GUI own presentation surfaces.
- Profiles supplies identity and disclosure semantics.
- The host/provider enforces access, delivery, cache invalidation, and device permissions.

CreatorNetwork does not author a creator's studio, implement painting tools, simulate clay, or execute a venue.

## 8. Advertising remains a separate future concern

Diegetic advertising is an exploratory direction, not an implemented CreatorNetwork capability. A future system must distinguish placement, rendering, viewability, peripheral exposure, direct focus, interaction, and occlusion. Presence in a scene is not proof that a person saw or engaged with an object.

Any measurement and billing must use contractually agreed, auditable events. Do not assume eye tracking, infer attention from device sensors, or perform covert behavioral profiling. Creator opt-in and intensity controls, host policy, user disclosure, privacy, and applicable device permissions must be independent gates. The advertising domain and Economy should own campaign measurement and billing; CreatorNetwork should not grow an ad-tech subsystem by accident.

## 9. Current implementation status

Implemented in the provider-neutral Contracts assembly:

- `ProvenanceReferenceKind` and `ProvenanceReference`;
- immutable, defensively snapshotted `ProvenanceRecord`;
- `ProvenanceGraphValidator` structural checks for required fields, duplicate IDs/outputs, self-reference, and cycles;
- `RightsDeclaration` and its validator for exact-resource scope, explicit action grants, claim status, and validity windows;
- `RepresentationOffer` and its validator for source-to-alternative mappings that reference separate provenance and rights records;
- focused tests for a source-to-approved-derivative chain, invalid metadata, cycles, duplicate outputs, and defensive snapshots.

Not implemented:

- provenance persistence, signatures, tamper-evident storage, external evidence verification, identity proof, completeness proofs, rights/license evaluation, identity/authority verification, source-owner representation resolution, cache invalidation, claimant portal, royalty accounting, or ad measurement;
- changes to the existing `creator-network.channel/1` wire contract;
- automatic import of remote source URIs or remote graph traversal.

The cross-reference overload of `RepresentationOfferValidator.Validate(offer, provenance, rights)` additionally checks that the supplied provenance record is the referenced record, its output is the exact alternative artifact, the offered source artifact appears as an input, and the supplied rights declaration is the referenced declaration scoped to that exact alternative. Disputed or withdrawn declarations cannot support an active offer. These checks prevent accidental cross-wiring of records; they still do not prove publisher authority or authorize a request.

The validators are structural safety nets, not a trust engine. Do not publish or consume a restricted resource merely because its provenance graph or representation offer passes validation.

## 10. Next acceptance proof

The first end-to-end museum proof should use two independent museum publishers and one consumer Experience:

1. Museum A publishes an original artifact, its source-work identity, and a separately governed building/collection resource.
2. Museum A publishes an approved low-complexity substitute with explicit lineage from the source artifact and a distinct rights declaration.
3. The consumer requests the semantic centerpiece slot. The resolver selects only a compatible, authorized representation.
4. A restricted original is never fetched or transformed by the consumer as a fallback.
5. Museum B supplies an alternate collection for the same semantic slots. The Experience swaps sources without rebuilding its composition.
6. A rights change or takedown invalidates controlled cached resolutions and prevents new delivery; audit records show what was invalidated and what is outside provider control.
7. A composite derived from multiple works retains every input edge and does not infer royalty entitlement without an explicit agreement.

Until that proof exists, these contracts are an initial lineage foundation, not a production provenance or licensing system.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
