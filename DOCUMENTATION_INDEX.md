![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork documentation

Use this index to find the right level of explanation. The README introduces the user problem and payoff; these documents define concepts, boundaries, and the working path.

## Start here

| Repository guidance | [CreatorNetwork LLM contract](CreatorNetworkLLM.md) · [Domain and trust review](docs/DOMAIN_REVIEW.md) · [Creator ecosystem model](docs/CREATOR_ECOSYSTEM_MODEL.md) |

| If you want to… | Read |
|---|---|
| Understand the problem CreatorNetwork solves | [Theory learning path](docs/theory/README.md) · [Theory overview](docs/THEORY.md) |
| See who owns each responsibility | [Architecture](docs/ARCHITECTURE.md) |
| Understand the single publication contract | [Publication contracts](docs/PUBLICATION_CONTRACTS.md) |
| Understand provenance, source-work identity, museum reuse, approved representations, creator venues, royalties, and exploratory advertising | [Creator ecosystem model](docs/CREATOR_ECOSYSTEM_MODEL.md) · [Provenance and rights-aware representations](docs/PROVENANCE_AND_REPRESENTATIONS.md) |
| Integrate profile identity and disclosure safely | [Profiles Integration](docs/PROFILES_INTEGRATION.md) |
| Upload and retrieve a local recorded-media asset | [Local Media Assets](docs/MEDIA_ASSETS.md) |
| Understand creator authoring, capture, recorded media, and live streaming boundaries | [Creator Studio, Capture, and Streaming](docs/CREATOR_STUDIO_AND_STREAMING.md) |
| Follow the intended end-to-end proof | [Getting Started](docs/GETTING_STARTED.md) · [Museum representation consumer sample](samples/CreatorNetwork.MuseumConsumer/README.md) |
| Compare interoperability approaches | [Interoperability](docs/INTEROPERABILITY.md) |
| Understand security, trust, and moderation limits | [Security and Trust](docs/SECURITY_AND_TRUST.md) |
| Understand how performance is measured | [Performance](docs/PERFORMANCE.md) |
| Contribute without breaking boundaries | [Development](docs/DEVELOPMENT.md) |

## The central model

CreatorNetwork is the creator-facing **Content Creator platform** in The Singularity Workshop. It enables cross-platform creation by coordinating creator workflows across tools, devices, media, services, data, simulations, MicroBundles, and Experiences. Its core technical responsibility is the portable **distribution, publication, provenance/rights, and trust plane** connecting stable channel/publication references to replaceable repositories, discovery providers, and independent hosts. MicroBundleDomain defines capability meaning; MicroBundleRepository supplies artifact retrieval; FSM_COS composes verified MicroBundle capabilities; Experiences owns authored composition and build/finalization; UserIO and host-owned adapters handle semantic interaction and source acquisition; the host owns execution policy and manifestation. Media is one resource family, not the root abstraction. Profiles supplies identity/disclosure semantics, while trusted hosts/providers enforce authentication and authorization. The current channel/media endpoints remain a local proving slice, not proof that the general artifact/Experience lifecycle is complete.

## Documentation quality rule

Documentation explains the user problem and payoff first, then practical use, examples, ownership boundaries, and deliberate exclusions. Architecture and theory are living explanations of the intended system—not private design notes, CI transcripts, or unresolved TODO lists. Implementation claims must be verified against code and runnable examples.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
