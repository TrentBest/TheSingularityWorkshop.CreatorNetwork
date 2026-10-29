![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork documentation

Use this index to find the right level of explanation. The README introduces the user problem and payoff; these documents define concepts, boundaries, and the working path.

## Start here

| Repository guidance | [CreatorNetwork LLM contract](CreatorNetworkLLM.md) · [Domain and trust review](docs/DOMAIN_REVIEW.md) |

| If you want to… | Read |
|---|---|
| Understand the problem CreatorNetwork solves | [Theory learning path](docs/theory/README.md) · [Theory overview](docs/THEORY.md) |
| See who owns each responsibility | [Architecture](docs/ARCHITECTURE.md) |
| Integrate profile identity and disclosure safely | [Profiles Integration](docs/PROFILES_INTEGRATION.md) |
| Upload and retrieve a local recorded-media asset | [Local Media Assets](docs/MEDIA_ASSETS.md) |
| Understand creator authoring, capture, recorded media, and live streaming boundaries | [Creator Studio, Capture, and Streaming](docs/CREATOR_STUDIO_AND_STREAMING.md) |
| Follow the intended end-to-end proof | [Getting Started](docs/GETTING_STARTED.md) |
| Compare interoperability approaches | [Interoperability](docs/INTEROPERABILITY.md) |
| Understand security, trust, and moderation limits | [Security and Trust](docs/SECURITY_AND_TRUST.md) |
| Understand how performance is measured | [Performance](docs/PERFORMANCE.md) |
| Contribute without breaking boundaries | [Development](docs/DEVELOPMENT.md) |

## The central model

CreatorNetwork is the portable **distribution, publication, and trust plane** for creator-authored MicroBundles, Experiences, and typed resources. It connects stable artifact/Experience references and versioned publication metadata to replaceable repositories, discovery providers, and independent hosts. MicroBundleDomain defines capability meaning; MicroBundleRepository supplies artifact retrieval patterns; FSM_COS composes verified MicroBundle capabilities; Experiences owns authored composition and build/finalization; the host owns execution policy and manifestation. Media is one resource family, not the root abstraction. Profiles supplies identity/disclosure semantics, while trusted hosts/providers enforce authentication and authorization. The current channel/media endpoints remain a local proving slice, not proof that the general artifact/Experience lifecycle is complete.

## Documentation quality rule

Documentation explains the user problem and payoff first, then practical use, examples, ownership boundaries, and deliberate exclusions. Architecture and theory are living explanations of the intended system—not private design notes, CI transcripts, or unresolved TODO lists. Implementation claims must be verified against code and runnable examples.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
