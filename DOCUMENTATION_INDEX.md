![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork documentation

Use this index to find the right level of explanation. The README introduces the user problem and payoff; these documents define concepts, boundaries, and the working path.

## Start here

| If you want to… | Read |
|---|---|
| Understand the problem CreatorNetwork solves | [Theory](docs/THEORY.md) |
| See who owns each responsibility | [Architecture](docs/ARCHITECTURE.md) |
| Integrate profile identity and disclosure safely | [Profiles Integration](docs/PROFILES_INTEGRATION.md) |
| Understand creator authoring, capture, recorded media, and live streaming boundaries | [Creator Studio, Capture, and Streaming](docs/CREATOR_STUDIO_AND_STREAMING.md) |
| Follow the intended end-to-end proof | [Getting Started](docs/GETTING_STARTED.md) |
| Compare interoperability approaches | [Interoperability](docs/INTEROPERABILITY.md) |
| Understand security, trust, and moderation limits | [Security and Trust](docs/SECURITY_AND_TRUST.md) |
| Understand how performance is measured | [Performance](docs/PERFORMANCE.md) |
| Contribute without breaking boundaries | [Development](docs/DEVELOPMENT.md) |

## The central model

CreatorNetwork separates **profile semantics**, **portable publication**, **content storage**, **discovery**, **delivery**, and **presentation**. Creator-facing tools own the authoring workflow; capture adapters and media providers own acquisition and delivery; live sessions have a lifecycle distinct from durable publications. The Profiles domain describes entities and observer-specific disclosure; CreatorNetwork publishes only an approved projection of that information alongside versioned publication metadata. No one provider is required to own every stage.

## Documentation quality rule

Documentation explains the user problem and payoff first, then practical use, examples, ownership boundaries, and deliberate exclusions. Architecture and theory are living explanations of the intended system—not private design notes, CI transcripts, or unresolved TODO lists. Implementation claims must be verified against code and runnable examples.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
