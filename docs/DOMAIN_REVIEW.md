![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork Domain Review — boundaries before expansion

This is an implementation-grounded review checkpoint for [Issue #7](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7). It is a working domain decision record, not a claim that CreatorNetwork is production-ready or that unresolved legal and trust questions have been solved.

**Scope correction:** CreatorNetwork is not a video/audio service. Its intended domain is the portable distribution, publication, discovery/addressing, integrity, provenance, rights-policy, and trust plane connecting creator-authored MicroBundles, Experiences, and typed resources to replaceable repositories/providers and independent hosts. Media is one resource family, not the root abstraction. The existing channel/media endpoints are a local proving slice; they do not yet prove the broader artifact-and-Experience lifecycle. Integrity verification also does not make executable content safe to load or execute.

## 1. Domain glossary

| Term | Meaning and authority |
|---|---|
| **Profile / persona** | Identity meaning, claims, relationships, and observer-specific disclosure owned by [Profiles](https://github.com/TrentBest/TheSingularityWorkshop.Profiles). A display name or profile URI is not authentication. |
| **Creator identity reference** | A portable reference intentionally exposed in a publication. It does not itself prove identity control, authorization, ownership, or permission to publish. |
| **Experience** | An authored, versioned composition of reusable capabilities and content that a host can validate and present or execute. The Experiences domain owns the authoring-to-runtime-composition workflow. |
| **MicroBundle** | A reusable unit defined by [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain). Reference reusable meaning by stable identity/version; do not copy it into every Experience without reason. |
| **Runtime manifest** | A request for capabilities and versions to compose. [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) consumes it to resolve and assemble runtime capabilities; it is not an editor project or publication manifest. |
| **Publication** | CreatorNetwork's versioned, portable declaration of an authored artifact, Experience, or resource made available under a stated policy, with stable references and revision semantics. The current channel DTO is a provisional implementation, not yet a complete general artifact publication contract. |
| **Media asset** | Durable or finalized bytes with content type, length when known, integrity evidence, and provider delivery references. |
| **Live session** | A time-bound session with its own authorization, endpoint, reconnect, and termination lifecycle. It is not interchangeable with a durable asset or publication revision. |
| **Source URI** | An untrusted location hint. It is not an identity, integrity proof, authorization grant, or guarantee that fetching is safe. |
| **Creator Node** | A provider implementation for storing/serving CreatorNetwork records and, where explicitly supported, media. The current local prototype is unauthenticated and development-only. |
| **Rights/provenance declaration** | A statement or evidence reference about attribution and permitted uses. Metadata alone does not prove legal ownership or permission. |
| **Host** | The application/service that supplies trusted identity and authorization decisions, chooses presentation, and owns application lifecycle. |

## 2. Actor and authority matrix

| Actor / system | May decide or provide | Must not be assumed to prove |
|---|---|---|
| Creator / publisher | Intended content, metadata, attribution assertions, selected audience, publication request | Legal rights solely by asserting them; authenticated identity merely by naming a profile |
| Authenticated principal | The identity established by the host's authentication mechanism | Permission to publish every asset or disclose every persona |
| Profiles + trusted host adapter | Identity semantics, persona resolution, audience-specific approved projection | Endpoint authorization unless the host explicitly enforces it |
| CreatorNetwork contracts | Publication schema, revisions, content references, validation rules | Authentication, legal permission, media availability, or moderation decisions |
| Experiences authoring workflow | Editability/validation, authored composition, deterministic finalized version, exclusion of development-only tooling | Canonical publication identity or permission to redistribute referenced assets |
| FSM_API | State-machine and lifecycle primitives | Application policy, authoring semantics, or distribution rights |
| FSM_COS | Dependency resolution and runtime capability composition | Editor validation, application scheduling/UI, creator registry, media delivery |
| MicroBundleDomain / Repository | MicroBundle semantics and artifact retrieval contracts | General recorded-media delivery policy or creator rights |
| UserIO / capture adapters | Explicitly selected capture source, permission result, captured output | Publication authorization or creator ownership |
| Media provider / cache | Bounded ingest, byte integrity, storage, range delivery, replication/cache policy | Creator identity or redistribution rights |
| Discovery provider | Indexing, search, feed/recommendation policy | Exclusive authority over which publications exist |
| Client / Experience consumer | User-facing rendering, accessibility, playback and user choice | Canonical ownership or permission to bypass origin access control |
| Moderator / rightsholder / provider operator | Provider-scoped reports, enforcement, takedown and appeals under declared policy | Universal authority across independent providers |
| Importing destination | Validate export, verify bytes, decide whether it is authorized to import/store/serve | Permission to copy merely because an origin is reachable |

## 3. Intended flow and trust boundaries

```text
Creator / author
   │ explicit source selection or authored input
   ▼
UserIO / import adapter ── permission/source result
   │ finalized source material; never silently capture
   ▼
Experiences authoring + validation
   │ deterministic, versioned runtime composition
   │ development-only editor/tooling excluded
   ▼
Host trust gate
   ├─ authenticate principal
   ├─ authorize publication and intended audience
   ├─ resolve approved Profiles persona/projection
   └─ evaluate rights/provenance and provider policy
   ▼
CreatorNetwork publication contract
   │ versioned metadata + stable content references
   ├──────────────► Discovery provider (index/reference only)
   ▼
Media provider / Creator Node
   │ streaming ingest, finalize, integrity check, access control
   ▼
Independent consumer / Experience
   │ resolve publication; independently authorize media read
   ▼
Export package + media inventory
   │ importer checks source trust, import permission, bytes, conflicts
   ▼
Second provider / host
   │ preserve portable publication identity and history
   ▼
Revocation / takedown / dispute
   ├─ deny future authorized reads at serving boundaries
   ├─ propagate policy notices to known replicas/caches where possible
   └─ report remaining copies and appeal state honestly
```

**Trust rules:** a hash proves byte equality, not authorship or legal permission. A visibility field is not access control. A source URI must not be fetched blindly; the importer needs an explicit origin-fetch policy and SSRF protections. Revocation cannot guarantee erasure of copies already lawfully or unlawfully downloaded; providers must report the scope of their control.

## 4. Ecosystem ownership and reuse decisions

| System | Decision | Evidence / boundary |
|---|---|---|
| Profiles | **Reuse through a narrow adapter** | Profiles owns identity/profile semantics, relationships, claims, and observer-specific disclosure. The host/provider still owns authentication and authorization enforcement. Publish only an approved projection, not a full profile. |
| UserIO | **Do not duplicate capture** | Inspected `SemanticIntent` is an application-owned intent boundary, not a universal camera/microphone/device-capture implementation. Consume finalized output from a verified host-owned adapter. |
| SingularityWarehouse | **Investigate a narrow reusable storage/cache contract; no direct coupling yet** | `LocalCacheService` persists data under LocalApplicationData and exposes blob operations, but writes are byte-array based. Caller-provided path components need confinement review; identity fallback and concurrency semantics are not a CreatorNetwork trust boundary. It does not currently establish streaming media, content-addressed immutable artifacts, retention, migration, or publication revocation. MicroBundle cache reuse should avoid repeat downloads when identity/version/digest match, without conflating executable artifacts with mutable media/resources. |
| MicroBundleDomain | **Reuse its canonical meaning** | Defines MicroBundle capability identity, version, dependencies/providers/configuration, loading, and arbitration. Keep the What boundary separate from publication/distribution and from FSM_COS composition. |
| MicroBundleRepository | **Reuse delivery/integrity patterns, not its domain as CreatorNetwork** | Provider-neutral artifact repository, explicit version + SHA-256 identity, and replaceable local/remote providers are useful patterns. Existing filesystem implementation materializes byte arrays and needs immutability/root-confinement review before any claim of direct reuse. A MicroBundle artifact is not a generic media/resource object. |
| FSM_API | **Reuse as a lifecycle primitive** | State-machine/context mechanics only; no CreatorNetwork policy belongs in FSM_API. Follow [FSM_API LLM guidance](https://github.com/TrentBest/FSM_API/blob/master/FSM_APILLM.md). |
| FSM_COS | **Use for runtime composition** | Resolves, validates, loads, and arbitrates MicroBundle compositions into a RuntimeAssembly. It does not download artifacts, verify repository hashes, edit content, schedule the application, or render an Experience. Never add editor metadata to `RuntimeManifest`. |
| Experiences | **Own Experience authoring and build/finalization** | Catalogs/manifests and packaging tools exist, but the inspected generic packer does not prove host editability, editor-capability validation, or exclusion of development-only editor roots. The missing authoring-to-finalized-runtime-composition seam belongs here; it is not a CreatorNetwork media feature or an FSM_COS responsibility. |
| StoryBuilder | **Reuse only where its actual domain fits** | The existing workspace/package compiler is not a generic substitute for Experiences' authoring/build contract. Do not repurpose a book-specific compiler to solve arbitrary Experience validation. |
| Foundry | **Reuse for general workflow orchestration** | If Hub-managed, Foundry owns sequencing, cancellation, run status, and structured outcomes. CreatorNetwork should expose narrow domain operations and remain usable headlessly. |
| GUI / The / Architect / AnyApp / WebApp | **Hosts and optional manifestations** | Presentation, interaction, host policy, and runtime lifecycle stay replaceable. A host must declare supported editors/handlers/capabilities; portability does not imply universal compatibility. |
| FSM_REST | **Evaluate only against a concrete transport need** | Inspect request/response and streaming contracts before introducing dependencies. |
| CiAi | **Use as CI authority when integrated** | Do not bypass required CI orchestration or infer domain correctness from a green build. |

The evidence above is source inspection recorded in [Issue #7](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7); it is not a production-readiness endorsement. Reuse architectural patterns only after the target contract, lifecycle, threat model, and tests are explicit.

## 5. Risk register

| Risk | Status | Required control / next decision |
|---|---|---|
| Unauthenticated local Node exposed beyond loopback | **Blocking for production** | Keep loopback-only; add authentication, authorization, quotas, TLS/reverse-proxy guidance, abuse controls, and threat tests before multi-user/public exposure. |
| Hash confused with ownership/rights | **Blocking domain misunderstanding** | Separate integrity from identity, provenance evidence, license declaration, and legal permission. Obtain qualified legal review where needed. |
| Unauthorized publication update or private read | **Blocking for production** | Enforce authorization at every serving boundary; visibility metadata alone is insufficient. |
| SSRF or hostile source reference during import | **Blocking for remote import** | Treat URI as untrusted; define allowed schemes/origins, redirect handling, private-network denial, size/time limits, and safe parsing. |
| Malicious uploads/parsers, quota exhaustion, path traversal | **High** | Bound streaming and parsing, safe storage names, quotas, cancellation, atomic finalization, and adversarial tests. |
| Stale cache, cache poisoning, or repeated downloads | **High** | Key caches by stable identity/version and verified content digest; define invalidation/revocation and trust provenance. Inspect SingularityWarehouse before choosing reuse. |
| Revocation cannot erase prior copies | **Intrinsic limitation** | Define future serving denial, replica/cache notification, audit and dispute states; never promise universal deletion. |
| Development editor shipped as runtime content | **Domain gap** | Experiences finalization must explicitly classify editor/tooling-only bundles and reject or exclude them from runtime composition. |
| Metadata migrates but media does not | **Known incomplete vertical slice** | Export a media inventory; independently verify bytes, missing assets, stable publication identity, and destination authorization. |
| Live session confused with recorded media | **Domain boundary** | Separate live-session lifecycle and endpoint authorization from durable asset finalization and publication revision. |
| Cross-repository contracts guessed from names | **Process risk** | Inspect actual source and current development branch before referencing types or adding dependencies. |
| ProtocolAi adopted without demonstrated need | **Avoidable complexity** | Evaluate only for semantic identity/reference validation; adopt only with a concrete contract and tests. |

## 6. Current implementation disposition

| Existing area | Disposition | Reason / constraint |
|---|---|---|
| `CreatorNetwork.Contracts` publication/content-reference DTOs and validator | **Keep provisionally; revise against the broader domain** | Useful provider-neutral wire types, but current channel/media fields and validation do not yet define a general Experience/artifact publication lifecycle, finalized-reference linkage, monotonic revision policy, rights decision, or host authorization. |
| Loopback file-backed Creator Node | **Keep as a local development prototype** | Useful HTTP/persistence proving surface. It is unauthenticated and not a production network or private-audience boundary. Keep loopback-only. |
| Streaming media ingest, SHA-256, finalized descriptor, range delivery | **Keep as one resource-provider fixture** | Useful bounded streaming and integrity primitives, but they do not define CreatorNetwork's root domain. Source review found normal exception cleanup in code, while tests do not assert all temporary-file cleanup paths; crash orphan sweeping is not established. |
| Publication-to-resource linkage and revisions | **Revise after policy decisions** | Source trace confirmed publication writes do not resolve references against finalized asset descriptors, enforce monotonic revisions, or retain prior revisions. These are contract gaps, not merely documentation gaps. |
| Profiles adapter | **Keep provisionally** | The adapter is a narrow integration experiment; identity projection/disclosure does not itself authenticate the caller or authorize publication. Require CI and fail-closed projection tests. |
| Experience authoring/build workflow | **Missing domain seam; upstream in Experiences** | Current packaging does not establish editability support, unsupported-content rejection, deterministic finalization, or editor-only root exclusion. Do not try to fix this by adding editor fields to FSM_COS RuntimeManifest. |
| Current README and theory/architecture docs | **Keep and reconcile** | The docs name the ecosystem boundaries, but the implementation and current runnable slice remain channel/media-centered. End-user claims must match code and tested behavior. |
| Benchmark scaffolding | **Defer expansion** | Select a domain-approved Experience distribution/composition workload first. Then measure end-to-end resolution, integrity, caching, allocations, and latency. |
| New endpoints, federation, custom identity/rights protocols, monetization | **Defer** | Require a concrete behavior, fit analysis, domain decision, and interoperability tests before implementation. |
| Full creator studio inside CreatorNetwork contracts | **Do not implement** | UI/editor belongs in a host or Experience; capture and media tooling remain replaceable capabilities. |
| Editor fields in FSM_COS.RuntimeManifest | **Do not implement** | Runtime composition manifest and authoring project are distinct domain objects. |

## 7. Proposed minimum vertical slice

The first proof should exercise the intended general distribution/composition domain. Keep it local and deliberately small:

1. Publish one versioned **Experience artifact** that references a small runtime composition: at least two MicroBundle capabilities with one declared dependency, plus one non-executable typed resource (for example an image or mesh fixture).
2. Keep the Experience artifact, MicroBundle artifacts, and typed resource distinct; do not assume an Experience is itself a MicroBundle or that every resource is executable.
3. Resolve artifacts through a provider-neutral repository/catalog boundary. Verify stable identity, explicit version, and SHA-256 before content is accepted; persistent caching may reuse a verified artifact only when its identity/version/digest match.
4. Use FSM_COS to compose the requested MicroBundle capabilities after retrieval/integrity verification. Demonstrate a minimal independent host manifestation, but do not execute arbitrary third-party code solely because its hash matches.
5. Validate the Experience's authoring/build path in Experiences: reject unsupported content with actionable diagnostics, deterministically finalize/version the runtime composition, and exclude editor-only/development tooling unless independently required at runtime.
6. Apply a test host's explicit authorization seam to publication and artifact fetch. Keep creator identity projection, rights/provenance claims, and authorization as distinct concepts.
7. Demonstrate a recorded-media asset as an optional resource fixture if useful; it must not define the root abstraction.
8. Export/import the Experience plus its artifact/resource inventory to a second local provider, preserve portable identity/version information, report missing references, and verify all transferred digests.
9. Demonstrate fail-closed behavior for missing dependencies, digest mismatch, unauthorized fetch, and unsupported authoring input. Record revocation/takedown limitations for provider-controlled origins/caches separately; do not promise recall of independent copies.

## 8. Acceptance tests (initial ten)

These are proposed observable gates, not claims that the current implementation passes them.

1. **Artifact identity and integrity:** a versioned artifact with a mismatched digest is rejected before it enters a trusted cache or composition catalog.
2. **Dependency resolution:** a valid Experience resolves all declared MicroBundle dependencies; a missing or incompatible dependency produces a stable diagnostic and no partial accepted composition.
3. **Typed resource distinction:** a non-executable resource can be referenced and integrity-checked without being misclassified as a MicroBundle or executed as code.
4. **Authoring editability:** unsupported or uneditable Experience content is rejected with an actionable diagnostic; no invalid finalized artifact is emitted.
5. **Editor/runtime separation:** editor-only/development-only MicroBundles are absent from the finalized runtime composition unless explicitly and independently declared as runtime-required.
6. **Deterministic finalization:** identical canonical inputs and pinned dependency versions yield the same composition identity/digest, or nondeterministic fields are explicitly excluded/versioned.
7. **Authorization and disclosure:** unauthorized publication/fetch is denied by the host/provider seam; absent or audience-incompatible Profiles projections fail closed without leaking private identity fields.
8. **Independent consumer/host:** a separate client resolves the published Experience, verifies referenced artifacts/resources, composes via FSM_COS, and produces a minimal manifestation without private CreatorNetwork implementation knowledge.
9. **Cache and migration correctness:** cache hits require matching stable identity/version/digest; export/import preserves publication identity, reports missing artifacts, verifies bytes, and rejects unauthorized imports.
10. **Revocation and unsafe inputs:** provider-controlled reads stop when access is revoked; hostile identifiers/path traversal and unapproved remote fetch targets fail closed; known cache/replica limitations are reported honestly.

## 9. Open decisions before production

- Which existing identity/authentication, authorization, federation, object-storage, attribution/rights, media-delivery, and takedown standards fit the requirements?
- What minimum provenance/license fields are required for publication versus copying/migration, and which claims require evidence or human review?
- What are the host's source-fetch rules and origin trust model for import?
- What narrow provider-neutral cache/storage contract is needed to reuse verified MicroBundle artifacts without directly coupling CreatorNetwork to SingularityWarehouse internals? Which mutability, streaming, authorization, retention, and revocation rules differ for media/resources?
- What precise finalized Experience artifact/manifest contract should Experiences own, and what is the smallest missing seam for editability validation, deterministic finalization, and editor exclusion?
- What provider-specific rules govern moderation, appeals, replica notification, and retention?
- Which questions require qualified legal review? Metadata must not be presented as a universal legal-compliance solution.

---

## Resources & Support

- [CreatorNetwork source](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork)
- [Issue #7 — domain and trust review](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7)
- [FSM_COS documentation](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)
- [FSM_API LLM coordination contract](https://github.com/TrentBest/FSM_API/blob/master/FSM_APILLM.md)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
