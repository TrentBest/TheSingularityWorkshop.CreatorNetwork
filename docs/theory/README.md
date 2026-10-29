![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork theory — learning path

This path explains CreatorNetwork's domain alongside the [operational guide](../GETTING_STARTED.md), [architecture map](../ARCHITECTURE.md), and [domain/trust review](../DOMAIN_REVIEW.md). It does not replace API reference or imply planned capabilities are implemented.

## Learn in order

1. [The problem and domain boundary](01-domain-and-problem.md) — why portability needs a separate publication plane.
2. [Publication contracts and references](02-publication-contracts.md) — what the current contract represents and what it does not.
3. [Trust, identity, and rights](03-trust-and-rights.md) — separate claims that are often incorrectly collapsed.
4. [A vertical slice and its evidence](04-workflow-and-evidence.md) — reason from code/tests to honest capability claims.
5. [Creator workflow and portability layers](05-creator-workflow-and-portability.md) — follow the intended creator journey and distinguish metadata, media bytes, identity, and serving policy.

## Current implementation boundary

The current channel schema is a channel-publication slice with creator and content references plus validation. The repository also contains a local Creator Node, authoring client, and Profiles integration. Those do not prove general Experience/MicroBundle publication, production identity, complete remote import/export, or public hosting.

## Evidence rule

Each chapter links to implementation and tests where available. Source proves what code says; a test proves only the behavior it exercises. Neither proves production deployment, lawful rights, universal interoperability, or untested security properties.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
