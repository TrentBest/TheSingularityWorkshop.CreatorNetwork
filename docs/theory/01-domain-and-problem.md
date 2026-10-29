![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# 1. The problem CreatorNetwork owns

## Why separate publication from platforms?

A single platform often combines identity, content storage, publication metadata, search, recommendations, and the client experience. That is convenient until a creator needs to replace one provider: identity, publication history, media links, and audience relationships can become coupled to that provider.

CreatorNetwork's goal is to make those responsibilities interoperable and replaceable. Its domain is the **portable distribution, publication, discovery/addressing, integrity, provenance, rights-policy, and trust plane** for creator-authored MicroBundles, Experiences, and typed resources. Media is one resource family, not the definition of the system.

## The boundary model

- **CreatorNetwork** owns publication records, stable references, revision semantics, publication-facing metadata, and provider-neutral distribution/discovery contracts.
- **Profiles** owns profile/persona meaning and observer-specific disclosure. A publication may carry an approved projection; a display name or profile URI is not authentication.
- **Experiences** owns authored composition and the authoring-to-finalized-runtime workflow.
- **MicroBundleDomain** owns MicroBundle meaning. **MicroBundleRepository** owns artifact retrieval patterns.
- **FSM_COS** resolves and assembles runtime capabilities. A runtime manifest is not a CreatorNetwork publication manifest.
- **The host/provider** owns authentication and authorization enforcement, storage/delivery implementation, operational availability, and host-specific presentation policy.
- **GUI** may present a workflow but is not the authority for publication state or access control.

These boundaries let a publication reference a reusable artifact without becoming its semantic owner, and let many clients consume a publication without dictating how each client renders it.

## Portability has several dimensions

1. **Identity:** the creator can establish control independently of one hosting account.
2. **Publication:** metadata, references, and revision history can be exported and imported.
3. **Content:** the creator can lawfully and practically move or replicate bytes.
4. **Discovery:** multiple indexes and clients can find the same publication under different policies.
5. **Operations:** another provider can serve it without rebuilding the record by hand.
6. **Audience relationships:** relationships may be transferred or re-established only with appropriate consent and policy.

A metadata export alone is not complete portability. Moving bytes does not prove identity control, rights, authorization, or permission to execute content.

## Implemented versus intended

The current contract is explicitly a channel-manifest slice; the domain review states that the broader artifact-and-Experience lifecycle is not yet proven.

**Evidence:** [domain review](../DOMAIN_REVIEW.md), [architecture](../ARCHITECTURE.md), [theory overview](../THEORY.md), and [ChannelPublication.cs](../../src/CreatorNetwork.Contracts/ChannelPublication.cs).

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
