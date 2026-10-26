![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Interoperability and Protocol Choices

CreatorNetwork should interoperate with existing protocols where they solve the problem well. A creator-owned network should not become a proprietary island because it has a new repository.

## Evaluate by responsibility

| Concern | What to evaluate |
|---|---|
| Federated activity and notifications | ActivityPub and its ecosystem |
| Portable account identity, data repositories, and client/service interoperability | AT Protocol and its ecosystem |
| Creator-controlled identity and publication authenticity | Established signing, key-rotation, and identity practices |
| Media metadata and delivery | Common media metadata, HTTP semantics, byte-range support, established streaming formats |
| Export and migration | Machine-readable manifests, media inventories, stable identifiers |

These are evaluation starting points, not a claim that one protocol supplies every required capability or should be adopted wholesale.

## Decision criteria

Evaluate independent implementations, identity continuity and recovery, export/import, provider migration, moderation and abuse handling, privacy, content addressing versus mutable URLs, extensibility, operational complexity for a single creator, and rights/attribution requirements.

## Avoid premature protocol invention

Write down required user-visible behaviors and test cases first. Compare existing standards against those requirements. If an extension is necessary, document why it is needed and how implementations can ignore or preserve unknown fields safely.

A protocol is useful when independent implementations exchange real data. Prefer a small compatibility proof over an abstract claim of standards compliance.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
