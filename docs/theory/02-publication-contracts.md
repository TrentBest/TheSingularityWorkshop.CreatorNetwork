![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# 2. Publication contracts and references

## A publication is metadata, not a media server

The current ChannelPublication contract describes a versioned channel: schema, stable publication ID, creator projection, title/description, declared visibility, revision, update timestamp, ordered content references, and optional attribution. Its schema identifier is creator-network.channel/1.

The constructor snapshots the content-reference list into a read-only collection. That prevents a caller from replacing entries in the original list after construction; it does not freeze external storage or guarantee referenced bytes remain available.

## Identity and location are different

CreatorIdentity contains a stable opaque ID and optional display name/profile URI. Its source comments explicitly say this is a publication-safe projection—not a Profiles object and not proof of authentication.

ContentReference contains a stable opaque content ID, media type, optional byte length, optional SHA-256 digest, and optional source URI. The ID is intended to remain independent of a delivery URL. A source URI is only a location hint: it can become unavailable or point somewhere untrusted while logical content identity remains stable.

A SHA-256 digest can help verify byte equality when the bytes are obtained and hashed. It does not prove authorship, legal rights, safety, or permission to redistribute or execute those bytes.

## Validation is a contract gate, not a policy engine

PublicationValidator.Validate checks the current schema, required identifiers/title, positive revision, zero-offset timestamp, allowed visibility values (public, unlisted, private), required content IDs/media types, nonnegative known byte lengths, and the format of an optional SHA-256 digest. It returns actionable field-coded errors; an empty list means the validator accepted the record.

Validation does not authenticate the publisher, authorize an audience, enforce visibility at an HTTP endpoint, fetch a source URI safely, prove rights, moderate content, or ensure a host can deliver referenced bytes. Those concerns belong at their respective trust boundaries.

## Source evidence

- [ChannelPublication.cs](../../src/CreatorNetwork.Contracts/ChannelPublication.cs)
- [CreatorIdentity.cs](../../src/CreatorNetwork.Contracts/CreatorIdentity.cs)
- [ContentReference.cs](../../src/CreatorNetwork.Contracts/ContentReference.cs)
- [PublicationValidator.cs](../../src/CreatorNetwork.Contracts/PublicationValidator.cs)
- [PublicationValidatorTests.cs](../../tests/CreatorNetwork.Contracts.Tests/PublicationValidatorTests.cs)

Read validation tests as the exact tested contract, not as a substitute for integration/security testing.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
