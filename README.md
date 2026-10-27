![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork — A creator-owned publishing network

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Repository](https://img.shields.io/badge/GitHub-CreatorNetwork-181717?logo=github)](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork)

**Creators should own their identity, publications, and audience relationships. Viewers should choose how they discover and experience them. Hosting and discovery providers should be replaceable.**

CreatorNetwork is the foundation for a creator-owned internet: not another destination that asks everyone to move into one platform, but interoperable contracts and reference implementations that let independent creators publish once and reach many clients without surrendering the relationship to a single platform.

The repository contains the first provider-neutral contract library, validation tests, a loopback-only file-backed Creator Node prototype, named performance benchmarks, and a source-integrated Profiles publication adapter with focused tests. The adapter requires a trusted host resolver for publisher/persona selection and audience-specific disclosure; test execution remains pending. Independent publisher/consumer programs and the complete export/import path remain in progress; we distinguish working code from planned capabilities.

**[Start with the concept](docs/THEORY.md) · [Understand the architecture](docs/ARCHITECTURE.md) · [First vertical slice](docs/GETTING_STARTED.md) · [Local media assets](docs/MEDIA_ASSETS.md) · [Creator studio, capture, and streaming](docs/CREATOR_STUDIO_AND_STREAMING.md) · [Profiles integration](docs/PROFILES_INTEGRATION.md) · [Documentation index](DOCUMENTATION_INDEX.md)**

## 🟣 01 — Why CreatorNetwork?

A centralized platform commonly bundles identity, publishing, storage, discovery, recommendations, audience relationships, monetization, and playback. That is convenient, but makes creators dependent on one platform’s policies, reach, and continued existence.

CreatorNetwork separates those concerns:

- **Creator-owned identity:** identity is not merely an account at one hosting provider.
- **Portable publications:** channels and manifests reference content without tying it to one app.
- **Replaceable hosting:** content can move or replicate without rebuilding creator identity.
- **Independent discovery:** clients and communities choose discovery and recommendation policies.
- **Many manifestations:** one publication can appear in a website, desktop application, educational experience, virtual environment, or independent player.
- **Composable meaning:** MicroBundles compose meaning by reference, not duplication. Reusable content should be referenced rather than copied into every experience.

This is infrastructure for platform choice, not a claim that protocols alone solve economics, legal obligations, moderation, or distribution.

## 🟦 02 — What is the system?

At the center is a portable publication: creator identity, channel or collection, versioned metadata, and references to media or other content. Independent services may store, replicate, index, recommend, or deliver those publications. Clients decide how to present them.

<p align="center"><img src="docs/assets/creator-network-overview.svg" alt="Creator-owned identity and publication manifests connect to replaceable hosting and independent discovery; multiple clients consume the same publications." width="100%"></p>

The publication contract is the portable center. Storage, discovery, delivery, and presentation are providers around it—not a single mandatory platform.

## 🩵 03 — Where it fits in The Singularity Workshop

| Component | Responsibility |
|---|---|
| **CreatorNetwork** | Portable publishing contracts, creator-facing identity references, content references, discovery interfaces, and migration/export. |
| [Profiles](https://github.com/TrentBest/TheSingularityWorkshop.Profiles) | Shared domain semantics for entities, claims, relationships, and observer-specific disclosure. CreatorNetwork wire contracts carry only the approved publication-safe projection; they do not replace or serialize a full Profile. |
| [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain) | Domain contracts for independently meaningful MicroBundles. CreatorNetwork may reference this model where it fits; it does not redefine it. |
| [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) | Resolves and assembles runtime capabilities for a host. It is not the creator registry, media host, or recommendation authority. |
| Experiences and application hosts | Demonstrate and present publications; they consume CreatorNetwork contracts rather than own the network. |
| Media/storage providers | Store, replicate, transcode, and deliver media through replaceable provider boundaries. Executable artifact storage and media delivery remain distinct concerns. |

**Core principle:** Profiles owns profile meaning and disclosure semantics; CreatorNetwork owns publication semantics; providers implement replaceable services; clients choose how to manifest the result.

## 🟢 04 — See it in a minute

The first proof is intentionally smaller than a social network:

1. Create a stable creator identity.
2. Publish a versioned channel manifest with metadata and content references.
3. Serve the publication from a local or HTTPS-backed Creator Node.
4. Discover and render it from a separate client.
5. Export the publication and move it to another host without changing creator identity.
6. Consume the same publication from a separate Experience without hardcoding its content.

The contract models, validator, and local Creator Node prototype are implemented. The Profiles publication adapter now gates on an authenticated principal, validates the audience identity, delegates trusted persona resolution to a host-provided resolver, and fails closed when no approved representation is returned. Its tests have not yet been executed. Independent publisher/consumer programs and export/import are still to come; the six-step workflow is an acceptance target, not a claim that it already runs.

## 🟪 05 — Build the open network

- **Understand the why:** [Theory](docs/THEORY.md).
- **Understand ownership and boundaries:** [Architecture](docs/ARCHITECTURE.md).
- **Follow the first proving path:** [Getting Started](docs/GETTING_STARTED.md).
- **Integrate profile identity and disclosure safely:** [Profiles Integration](docs/PROFILES_INTEGRATION.md).
- **Upload and retrieve a local recorded-media asset:** [Local Media Assets](docs/MEDIA_ASSETS.md). This prototype is loopback-only and not production-secure.
- **Understand authoring, capture, recorded media, and live-stream boundaries:** [Creator Studio, Capture, and Streaming](docs/CREATOR_STUDIO_AND_STREAMING.md).
- **Review protocol choices:** [Interoperability](docs/INTEROPERABILITY.md).
- **Understand trust limits:** [Security and Trust](docs/SECURITY_AND_TRUST.md).
- **Measure performance:** [Performance](docs/PERFORMANCE.md).
- **Contribute deliberately:** [Development](docs/DEVELOPMENT.md) and [Documentation Index](DOCUMENTATION_INDEX.md).

Keep the portable contract layer dependency-light. Provider-specific integrations belong behind explicit interfaces and must not force every creator or client to use the same vendor. Performance improvements should be measured against stable workloads, with allocations and end-to-end latency considered alongside throughput.

---

## Resources & Support

- **Source:** [CreatorNetwork on GitHub](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork)
- **The Singularity Workshop:** [GitHub profile](https://github.com/TrentBest) · [Coder Legion](https://coderlegion.com/user/The%2BSingularity%2BWorkshop)
- **Community:** [Coder Legion](https://coderlegion.com/)
- **Support:** [Patreon](https://www.patreon.com/c/TheSingularityWorkshop) · [PayPal](https://www.paypal.com/donate/?hosted_button_id=3Z7263LCQMV9J)

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
