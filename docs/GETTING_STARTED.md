![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Getting Started: the first vertical slice

The first milestone should prove creator portability end to end. It should not attempt to ship a complete social network, video editor, streaming platform, or recommendation engine.

> **Status:** this is the intended proving path, not a claim that implementation code already exists.

## The outcome

A creator publishes a channel and media reference. A separate client discovers and displays that publication. The creator exports it and imports it into a different host without changing the creator identity.

## The six steps

1. **Create an identity.** Establish a stable creator identifier that is not simply the username at one host.
2. **Publish a channel.** Write a versioned manifest with title, description, attribution, visibility declaration, and media references.
3. **Serve it from a Creator Node.** Start with a local implementation, then serve the same records over HTTPS.
4. **Consume it independently.** A separate client loads the manifest and renders the channel without relying on a private in-process object graph.
5. **Export and import.** Export the manifest and metadata, inventory referenced media, and import to a second origin.
6. **Verify continuity.** Confirm the creator identity remains stable and the second client can render the imported publication.

## Acceptance criteria

- Publisher and consumer are separate programs or processes.
- Publication uses a documented, versioned contract.
- Consumer works without a hard-coded creator/channel record.
- Export/import preserves identity references and publication versions.
- Media portability is tested separately from metadata portability.
- Missing media and unsupported manifest versions produce actionable errors.
- At least two origins can serve the same publication contract.
- A showcase experience consumes the publication through the contract rather than embedding content directly.

## Keep the first implementation boring

Use local files for the initial development store and HTTP(S) for the first network boundary. Avoid requiring cloud credentials, a particular database, or one media provider just to run the sample. A small openly licensed test asset or synthetic fixture is sufficient.

A creator computer is not automatically a dependable public origin. Public service needs reachable hosting, a relay, or another explicit availability plan. Keep that fact visible in sample documentation.

## What comes after the slice

After the portability proof works, add one concern at a time: signed publications, explicit permission enforcement, multiple discovery providers, subscription export, media replication, and improved streaming. Each addition should have a concrete consumer and a testable contract.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
