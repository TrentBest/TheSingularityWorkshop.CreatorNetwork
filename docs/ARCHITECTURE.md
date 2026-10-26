![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# CreatorNetwork Architecture

This document describes the intended system boundary. It distinguishes portable contracts from provider implementations and client experiences so that this project does not recreate a centralized platform under a different name.

<p align="center"><img src="assets/creator-network-overview.svg" alt="CreatorNetwork contracts connect creators to replaceable content hosts and discovery providers, then to independent clients."></p>

## 1. Responsibility map

| Boundary | Owns | Does not own |
|---|---|---|
| Creator identity | Stable identity reference and proof-of-control boundary | Hosting account as the only identifier |
| Publication contracts | Channel metadata, versions, content references, attribution, visibility declarations | Transcoding, storage engines, or UI |
| Creator Node | Self-hostable implementation for publishing and serving records | Mandatory global registry or exclusive host |
| Content provider | Media bytes, object identity, availability, replication, delivery | Creator identity or discovery policy |
| Discovery provider | Indexing, search, feeds, recommendations under a declared policy | Exclusive authority over which publications exist |
| Subscription relationship | User-controlled relationship, with consent and privacy constraints | An unexportable provider-owned graph |
| Client / manifestation | Playback, presentation, accessibility, interaction, user choice | Canonical ownership of publication |
| Runtime composition | Assembles capabilities needed by a host when appropriate | Creator registry, media network, or universal recommendations |

These are logical boundaries, not a mandate for one package or service per row. Keep the first implementation small until a real consumer proves that a boundary needs independent deployment or versioning.

## 2. Publication flow

Creator and authoring tools create or update a versioned publication. A Creator Node serves it; content providers hold media bytes; discovery providers index references; export tooling moves the manifest and media inventory; independent clients consume it.

The publication references content. Providers may implement different storage and delivery mechanisms. No component should assume the same organization runs every provider.

## 3. Contract before implementation

The first contracts must answer:

- What stable identifiers represent creators, channels, publications, and content?
- How is a publication version represented and compared?
- Which fields are canonical, optional, or extension data?
- How does a publication reference media without binding to one provider?
- How are attribution, rights declarations, and visibility represented?
- What proves that an update was authorized by the creator?
- How does export preserve identity, versions, and content references?
- How do older clients handle unknown extensions?
- What happens when an origin, media reference, or discovery provider is unavailable?

These questions do not justify inventing a custom cryptographic or federation protocol before evaluating existing standards.

## 4. Control plane and media plane

Publication metadata and media bytes have different needs.

- **Control plane:** identity references, channel metadata, publication versions, permissions, provenance, and discovery records.
- **Media plane:** large files, byte-range requests, streaming formats, transcoding, caches, and replication.

The control plane should be compact, exportable, and versioned. The media plane should support ordinary web delivery first and allow specialized providers later. A reference to a video is not the video itself; metadata export is not proof that media is portable.

## 5. Relationship to the Workshop ecosystem

- **FSM_COS** may assemble capabilities for a host that consumes CreatorNetwork; it should not absorb identity, publishing, discovery, or media responsibilities.
- **MicroBundleDomain** remains authoritative for MicroBundle domain concepts. Use stable references and existing contracts where they add value.
- **Experiences** can demonstrate creator channels and alternative discovery/presentation experiences. It is a consumer and showcase, not the owner of portable publication infrastructure.
- **AnyApp, WebApp, and other hosts** can present the same publication differently.
- **Media storage and delivery** are not the same problem as immutable executable MicroBundle artifact storage. Similar identity and integrity techniques may be reusable, but contracts and threat models differ.

## 6. Reliability and migration

The first Creator Node can use local storage for development and an ordinary HTTPS origin for public access. Production availability requires reachable hosting, domain ownership, TLS, backups, replication, abuse response, monitoring, and recovery.

Migration should produce a portable export with a documented manifest and media inventory. Import validates the export, preserves identity references, reports unavailable assets, and avoids silently rewriting publication history. Content hashes help verify bytes but do not establish copyright ownership or permission to redistribute them.

## 7. Deliberate exclusions

CreatorNetwork is not a single official feed, mandatory centralized media host, complete creator studio, replacement for every existing social protocol, runtime composition kernel, executable artifact repository, or guarantee of anonymity, legal immunity, censorship resistance, or permanent availability.

The system succeeds when independent implementations interoperate and creators can move between them—not when every feature is implemented in this repository.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
