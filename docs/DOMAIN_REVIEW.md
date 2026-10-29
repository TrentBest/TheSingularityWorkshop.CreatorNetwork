![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork Domain Review — boundaries before expansion

This is an implementation-grounded review checkpoint for [Issue #7](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7). It is a working domain decision record, not a claim that CreatorNetwork is production-ready or that unresolved legal and trust questions have been solved.

## 1. Domain glossary

| Term | Meaning and authority |
|---|---|
| **Profile / persona** | Identity meaning, claims, relationships, and observer-specific disclosure owned by [Profiles](https://github.com/TrentBest/TheSingularityWorkshop.Profiles). A display name or profile URI is not authentication. |
| **Creator identity reference** | A portable reference intentionally exposed in a publication. It does not itself prove identity control, authorization, ownership, or permission to publish. |
| **Experience** | An authored, versioned composition of reusable capabilities and content that a host can validate and present or execute. The Experiences domain owns the authoring-to-runtime-composition workflow. |
| **MicroBundle** | A reusable unit defined by [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain). Reference reusable meaning by stable identity/version; do not copy it into every Experience without reason. |
| **Runtime manifest** | A request for capabilities and versions to compose. [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) consumes it to resolve and assemble runtime capabilities; it is not an editor project or publication manifest. |
| **Publication / channel** | CreatorNetwork's versioned, portable statement of what a creator makes available, including approved metadata and content references. |
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

| System | Decision | Evidence / boundary to verify before integration |
|---|---|---|
| Profiles | **Reuse through adapter** | Current `src/CreatorNetwork.Profiles` is source-integrated and delegates persona resolution/disclosure to a trusted host resolver. Do not serialize full Profiles state into public publication DTOs. |
| UserIO | **Reuse for capture** | Capture belongs to selected-source adapters with permission lifecycle. CreatorNetwork should accept finalized output, not reimplement device capture. Verify actual interfaces before adding a dependency. |
| SingularityWarehouse | **Investigate before reuse** | Inspect its actual disk/cache, identity, authorization, and lifecycle contracts. Persistent caching should avoid repeat MicroBundle downloads where identity/version/integrity match; do not assume media assets share executable-artifact semantics. |
| MicroBundleDomain / MicroBundleRepository | **Reuse compatible references; keep storage semantics distinct** | MicroBundleDomain owns bundle meaning; Repository owns retrieval/location concerns. Inspect current contracts and integrity patterns before adopting them for media. |
| FSM_API | **Reuse as lifecycle primitive** | State-machine/context mechanics only; no CreatorNetwork domain policy belongs in FSM_API. Follow [FSM_API LLM guidance](https://github.com/TrentBest/FSM_API/blob/master/FSM_APILLM.md). |
| FSM_COS | **Use for runtime composition when needed** | It resolves/assembles capabilities. Do not add editor metadata to `RuntimeManifest` or turn COS into an authoring, media, or application scheduler. |
| Experiences | **Own authored-to-finalized composition** | The missing domain is a workflow that checks editability with platform tooling or a supplied super/editor Experience, rejects unsupported content, deterministically captures and versions the runtime composition, and excludes development-only editors. This is a domain target, not yet a verified implementation claim. |
| Foundry | **Reuse for general workflow orchestration** | If Hub-managed, Foundry owns sequencing, cancellation, run status, and structured outcomes. CreatorNetwork exposes narrow domain operations; do not duplicate a general workflow engine. |
| GUI | **Optional presentation** | Host-selected UI only; keep headless authoring/validation possible. |
| FSM_REST | **Evaluate for transport integration** | Verify actual request/response contracts and call sites before introducing a dependency. Media bytes need streaming-capable paths. |
| CiAi | **Use as CI authority when integrated** | Do not bypass required CI orchestration or infer domain correctness from a green build. |
| The / Architect / AnyApp / WebApp | **Potential hosts and consumers** | Same finalized publication/composition workflow should be invocable by a host without CreatorNetwork owning the UI. Confirm concrete contracts before implementing adapters. |

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
| Provider-neutral contract library and validation | **Keep, review** | Useful portable center. Verify each field is publication semantics, not host/UI/provider policy. |
| Loopback file-backed Creator Node | **Keep as development prototype** | Useful persistence/HTTP proving surface; unauthenticated and not production-secure. |
| Streaming media ingest, SHA-256, finalized descriptor, range delivery | **Keep provisionally; test adversarially** | Useful media-plane primitives; integrity does not establish rights. Confirm cancellation, bounded memory, atomic finalization, access checks, and hostile-input behavior. |
| Host-neutral authoring client | **Keep provisionally** | Should consume already-finalized media and require a separate explicit publish call; must not imply it captures or grants permissions. |
| Profiles publication adapter | **Keep provisionally** | Host-owned resolver and fail-closed disclosure are the right boundary; test execution and real host authorization remain separate evidence. |
| Current README and theory/architecture docs | **Keep and reconcile** | Problem-first structure and ownership boundaries are useful. Claims and runnable instructions must match current code and tests. |
| Existing benchmark scaffolding | **Defer expansion** | First select a real, domain-approved end-to-end workload; then measure stable latency, throughput, and allocations. |
| New endpoints, custom federation/identity/rights protocols, monetization | **Defer** | Require standards/fit review and explicit domain decisions before implementation. |
| Full creator studio inside CreatorNetwork contracts | **Do not implement** | UI/editor belongs in a host or Experience; capture and media tooling remain replaceable capabilities. |
| Editor fields in FSM_COS.RuntimeManifest | **Do not implement** | Runtime composition manifest and authoring project are distinct domain objects. |

## 7. Proposed minimum vertical slice

Build one independently testable path before broadening features:

1. A host selects or imports one source and records permission/source failures explicitly.
2. An Experience authoring workflow validates that the project is editable by the available platform tooling or supplied editor Experience.
3. Unsupported content is rejected with actionable diagnostics; development-only editors/tooling are excluded from the finalized runtime composition.
4. Finalization deterministically records the composition identity, version, and referenced MicroBundle versions/content digests.
5. A trusted host authenticates the publisher, authorizes the intended publication/audience, resolves the approved Profiles projection, and records the rights/provenance assertions and unresolved status honestly.
6. CreatorNetwork publishes a versioned manifest referencing finalized assets; it does not embed large media bytes or transient credentials.
7. An independent consumer resolves the publication and fetches authorized media through a provider, using bounded streaming and integrity verification.
8. An export/import path moves metadata and a media inventory to a second local provider while preserving publication identity and verifying every available byte.
9. Revoke/takedown prevents future authorized reads at controlled serving boundaries and records which replicas/providers were notified; it does not claim to erase uncontrolled copies.

## 8. Acceptance tests (initial ten)

1. **Unsupported authoring input:** invalid or uneditable content is rejected with a stable diagnostic and cannot be finalized.
2. **Runtime/editor separation:** editor-only/development-only MicroBundles do not appear in the finalized runtime composition; required runtime dependencies do.
3. **Deterministic finalization:** the same declared inputs and pinned dependency versions produce the same canonical composition identity/digest, or explicitly document any nondeterministic field.
4. **Explicit publication authority:** unauthenticated and unauthorized principals cannot create/update a publication; an approved host resolver is required for persona projection.
5. **Fail-closed disclosure:** absent, invalid, or audience-incompatible identity projection publishes no private/canonical identity fields.
6. **Interrupted media ingest:** cancellation, truncation, digest mismatch, duplicate ID, or failed finalization never creates a readable finalized asset.
7. **Bounded safe media handling:** large asset ingest/read uses bounded memory; path traversal, hostile metadata, unsafe redirects, and prohibited source targets are rejected.
8. **Independent consumption:** a separate client reads a publication without hard-coded creator/channel identity and verifies the referenced bytes.
9. **Cross-provider migration:** export/import preserves publication identity/revision, reports missing assets, checks import authorization, and verifies byte digests.
10. **Revocation/takedown accounting:** future authorized reads are denied at controlled serving boundaries; known replica/cache notifications and unresolved copies are reported without claiming universal erasure.

## 9. Open decisions before production

- Which existing identity/authentication, authorization, federation, object-storage, attribution/rights, media-delivery, and takedown standards fit the requirements?
- What minimum provenance/license fields are required for publication versus copying/migration, and which claims require evidence or human review?
- What are the host's source-fetch rules and origin trust model for import?
- Which SingularityWarehouse cache/storage contracts can be reused safely for MicroBundles, and which media-specific semantics require a separate adapter?
- What does a finalized Experience manifest contain, and which current Experiences contracts already express editability, deterministic finalization, and editor exclusion?
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
