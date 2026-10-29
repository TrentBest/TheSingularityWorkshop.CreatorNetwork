![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork LLM — repository and implementation contract

This document is the coordination contract for AI-assisted work in CreatorNetwork. Treat repository-owner instructions and the domain review as binding. Do not infer permission to publish, deploy, launch, or expose a service from a green build.

## Branch and release policy

- Work directly on `development`; leave `master` unchanged.
- Keep only `master` and `development` as long-lived branches. Do not create persistent feature/fix/release branches.
- Do not publish NuGet packages, deploy, enable public hosting, or launch monetization without explicit maintainer authorization.
- A build, test, pack, artifact upload, benchmark result, or green CI run is not authorization to publish.
- Report the exact commit, tests/workflows executed, and outcomes. Distinguish verified results from unverified implementation and design intent.

## Mandatory domain gate

Before feature expansion, review [Issue #7](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7) and [the domain review](docs/DOMAIN_REVIEW.md). Keep the review evidence-based; inspect actual source contracts and development branches before introducing dependencies or assuming capabilities.

The missing domain is the Experiences-owned authored-to-finalized runtime-composition workflow: validate editability with available platform tooling or a supplied editor/super-Experience, reject unsupported content, deterministically capture/version the runtime composition, and exclude development-only editor tooling. Do not confuse this authoring domain with CreatorNetwork publication contracts or FSM_COS runtime manifests.

## Ownership boundaries

- **CreatorNetwork:** provider-neutral publication/channel contracts, revisions, approved identity references, attribution/rights declarations, and portable content references.
- **Profiles:** identity semantics, persona relationships, and observer-specific disclosure. A trusted host must enforce authentication and authorization; a profile projection is not an authorization grant.
- **Experiences:** authoring, editability validation, finalization, and runtime-composition packaging.
- **FSM_API:** state-machine and lifecycle primitives. Do not put CreatorNetwork policy or host-specific dependencies into FSM_API.
- **FSM_COS:** resolve and assemble MicroBundle-based runtime capabilities. Do not add editor metadata to `RuntimeManifest`; do not make FSM_COS the editor, media engine, scheduler, or application UI.
- **MicroBundleDomain / MicroBundleRepository:** MicroBundle meaning and artifact retrieval. Assess reusable identity/integrity/cache patterns, but do not assume executable artifacts and recorded media share storage or threat semantics.
- **UserIO:** source selection, capture, and permission lifecycle. Consume finalized capture output rather than rebuilding device capture in CreatorNetwork.
- **Foundry:** general Hub workflow sequencing, cancellation, status, and structured outcomes. CreatorNetwork provides narrow domain operations and remains usable headlessly.
- **GUI:** optional host-selected presentation; not the authoritative workflow or run state.
- **Media providers:** streaming ingest/delivery, byte integrity, replication, and storage. Hashes prove byte equality, not identity or rights.
- **Clients / Experiences:** consume and present publications; they are not canonical publication owners.

## Security and trust rules

- Treat source URIs as untrusted location hints, never as identity, authorization, or proof of integrity. Remote import must define origin-fetch policy and SSRF defenses.
- Enforce access control at the serving boundary. Visibility metadata alone does not protect private content.
- Separate authentication, authorization, identity resolution, audience disclosure, content integrity, provenance, copyright/license assertions, moderation, and takedown.
- A hash does not prove authorship, copyright ownership, or permission to redistribute.
- Keep the local Creator Node loopback-only and development-only until authentication, authorization, quotas, abuse controls, and a production threat review exist.
- Never claim revocation erases copies already downloaded outside a provider's control.
- Do not adopt ProtocolAi or custom identity/federation/rights protocols without a concrete fit analysis and tests.

## Development and verification

1. Read [the domain review](docs/DOMAIN_REVIEW.md) and the specific subsystem docs before changing behavior.
2. Inspect existing code, tests, CI conventions, and dependency source on `development`. Prefer existing contracts over guessed interfaces.
3. Keep the portable contract layer provider-neutral and dependency-light. Keep UI, capture, transcoding, provider SDKs, and general orchestration behind explicit boundaries.
4. Write focused regression/acceptance tests for behavior and failure modes. Test interrupted I/O, invalid metadata, authorization boundaries, integrity mismatch, and migration where applicable.
5. Run the exact relevant test projects and repository-standard CI. Report command, environment, outcome, and any tests not run.
6. Update end-user documentation when behavior changes. Lead with the user problem and payoff, provide runnable commands/examples, explain ownership boundaries and limitations, and avoid private planning notes or CI transcripts in user docs.
7. Measure performance only against a domain-approved workload. Consider throughput, allocations, and end-to-end latency; accept complexity for demonstrated, repeatable gains, not speculative optimization.
8. Keep documentation claims synchronized with actual implementation. Mark plans as plans and prototype/security limitations prominently.

## Documentation standard

Follow the [FSM_COS README and documentation style](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS): problem/payoff first; clear five-part README progression; an architecture visual where useful; a short, runnable “see it work” path; explicit ecosystem ownership; support links; and a final `Avatar.jpg` footer. All Markdown documents should start with the Workshop moniker and end with the standard Avatar.jpg footer. Keep the documentation index accurate and check links/assets when editing.

## Completion report

Every substantial work session should state:
- What changed and why.
- Exact commit SHA(s).
- Tests/workflows actually run and their outcomes.
- Known gaps, assumptions, and security limitations.
- Whether any external action was taken. No package publishing, deployment, public hosting, or launch occurs without explicit authorization.

---

## Resources & Support

- [CreatorNetwork README](README.md)
- [Domain and trust review](docs/DOMAIN_REVIEW.md)
- [Issue #7](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/issues/7)
- [FSM_COS documentation standard](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS)
- [FSM_API LLM contract](https://github.com/TrentBest/FSM_API/blob/master/FSM_APILLM.md)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
