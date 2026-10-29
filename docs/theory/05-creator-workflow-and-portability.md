![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Creator workflow and portability layers

This chapter connects the portable-publication model to a creator's practical workflow. It distinguishes the intended end-to-end experience from the narrower channel-and-media prototype currently implemented.

## The workflow a creator should experience

A portable creator workflow is a sequence of explicit contracts, not a single platform-specific upload button:

1. **Choose the publishing identity.** A trusted host resolves the authenticated publisher to the intended public persona. Profiles owns profile semantics and audience-specific disclosure; CreatorNetwork receives only the approved publication-safe identity projection. See [Profiles integration](../PROFILES_INTEGRATION.md).
2. **Describe the publication.** Authoring produces a versioned publication record with a stable publication identifier, creator reference, title, revision, UTC update timestamp, visibility declaration, content references, and optional attribution.
3. **Validate before acceptance.** The receiver checks the schema and required metadata. Validation is a contract check, not proof of identity, ownership, moderation, or permission.
4. **Store the publication.** A host persists the accepted record and provides a retrievable location. A current URL is a delivery hint; it must not become the only identity of the publication or its content.
5. **Resolve content references.** A consumer interprets stable content IDs and media metadata, then obtains bytes from a permitted provider. A reference may include a source URI, length, and SHA-256 digest, but those optional fields do not mean bytes have been fetched or verified.
6. **Present through an independent client.** A client decides how the publication is displayed. It should consume the contract rather than embed a particular creator or host's record into its own implementation.
7. **Move and verify.** A portability proof exports the publication, imports it into another origin, and verifies that stable IDs and revision semantics survive. Media-byte migration is tested separately from metadata migration.

The intended experience is provider-replaceable. The current repository does **not** yet demonstrate every step as a complete independent publisher/consumer system; see [Getting Started](../GETTING_STARTED.md) for the exact current slice and remaining acceptance criteria.

## What the current contract carries

`ChannelPublication` uses the versioned schema `creator-network.channel/1`. It holds a publication ID, creator reference, title and optional description, visibility declaration, positive revision, UTC timestamp, ordered content references, and optional attribution. `ContentReference` separates a logical content ID from an optional source URI and can carry media type, byte length, and SHA-256 metadata.

These distinctions are important:

- **Stable ID is not location.** A host URL can change while the logical publication or content identity remains stable.
- **Metadata is not payload.** A channel manifest describes content; it does not contain the media bytes.
- **Digest is not availability.** A SHA-256 value can help verify exact bytes when they are obtained, but it does not fetch them or make the source reachable.
- **Visibility declaration is not access control.** The contract accepts `public`, `unlisted`, or `private`; the serving host must enforce the corresponding policy.
- **Attribution is not rights verification.** A text statement is not proof that the publisher may distribute the work.

Source: [ChannelPublication.cs](../../src/CreatorNetwork.Contracts/ChannelPublication.cs), [ContentReference.cs](../../src/CreatorNetwork.Contracts/ContentReference.cs), and [PublicationValidator.cs](../../src/CreatorNetwork.Contracts/PublicationValidator.cs).

## Boundaries across the Workshop

| Responsibility | Authority |
|---|---|
| Creator identity, profile semantics, audience-specific disclosure | [Profiles](https://github.com/TrentBest/TheSingularityWorkshop.Profiles) |
| Publication IDs, channel schema, revisions, content references | CreatorNetwork |
| Reusable capability meaning | [MicroBundleDomain](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleDomain) |
| Immutable artifact retrieval and repository mechanics | [MicroBundleRepository](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository) |
| Capability resolution and composition | [FSM_COS](https://github.com/TrentBest/TheSingularityWorkshop.FSM_COS) |
| Authored Experience composition and finalization | [Experiences](https://github.com/TrentBest/TheSingularityWorkshop.Experiences) |
| Authentication, authorization enforcement, storage operations, and execution policy | Trusted host/provider integration |
| User-facing presentation | An independent client; GUI may help, but is not the publication authority |

These boundaries are intentional. CreatorNetwork should integrate neighboring authorities through their public contracts instead of copying their domain models into a competing source of truth.

## Evidence and acceptance

Use the operational guide to run the current .NET 8 contract tests and local node. Those tests provide evidence only for the cases they exercise; they do not establish production security or complete portability.

A complete portability claim needs at least:

- separate publisher and consumer programs or processes;
- consumer behavior that does not depend on a hard-coded creator/channel record;
- export and import that preserve stable references and publication revisions;
- a second origin serving the same publication contract;
- tests for missing media and unsupported schema versions;
- independent verification of media bytes where a digest is provided; and
- an Experience or other client that consumes the publication through the contract.

Until those checks exist and pass, describe the system as a local channel/media proving slice—not a production-ready decentralized creator network.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
