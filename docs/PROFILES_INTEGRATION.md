![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Profiles Integration: Identity Without Disclosure Leakage

CreatorNetwork should use **TheSingularityWorkshop.Profiles** as the Workshop's shared domain vocabulary for entity identity, claims, relationships, and observer-specific disclosure. CreatorNetwork owns the portable publishing protocol; Profiles owns the meaning and projection of profile information.

- Profiles repository: https://github.com/TrentBest/TheSingularityWorkshop.Profiles
- Profiles API map: https://github.com/TrentBest/TheSingularityWorkshop.Profiles/blob/master/docs/API.md
- Profiles theory: https://github.com/TrentBest/TheSingularityWorkshop.Profiles/blob/master/docs/THEORY.md

## The boundary

| Concern | Authority |
|---|---|
| Canonical entity/profile and claims | Profiles |
| Observer-specific permitted representation | Profiles, through its disclosure model |
| Private links between canonical identity and publisher/persona profiles | Trusted identity infrastructure using Profiles' identity-link contracts |
| Portable creator reference in a publication | CreatorNetwork contract |
| Channel/publication IDs, revisions, attribution, content references | CreatorNetwork |
| Authentication, proof of control, authorization enforcement, storage and transport | Trusted host/integration |
| Media bytes and delivery | Content provider |

A CreatorIdentity in a channel manifest is a **portable, publication-safe projection/reference**, not a replacement for Profile, ProfileRepresentation, or ProfileIdentityLink. Its Id must refer to the identity the creator intentionally publishes under. It must not silently expose a canonical identity behind a publisher persona.

## Required publication flow

1. Resolve the authenticated publisher to the correct Profiles entity through trusted host integration.
2. Select the intended public publisher/persona profile. Do not assume the canonical profile is the public identity.
3. Apply Profiles disclosure rules for the relevant audience and create an observer-appropriate representation. For a public publication, only intentionally public publisher information belongs in the public projection.
4. Map the approved projection into the CreatorNetwork wire contract. Copy only fields the publication contract explicitly supports; do not serialize the complete Profile, its claims dictionary, private identity links, or internal access records.
5. Validate and publish the resulting ChannelPublication.
6. Enforce visibility and authorization at the serving boundary. A filtered representation is not a substitute for endpoint, storage, or transport security.

For private or group-limited publications, the host must evaluate the actual observer and policy before returning protected metadata or content. The current Creator Node prototype is unauthenticated and does not enforce private/unlisted visibility; it is local-development-only.

## Identity links and external accounts

Profiles distinguishes a domain profile from a provider-qualified external identity reference. External account mappings are not proof of authentication and do not grant authorization. CreatorNetwork must not use an email address, display name, profile URL, or host-local username as the sole stable identity key.

Likewise, a publication URI or current hosting URL is a location, not the identity itself. Migration should preserve stable identity references while allowing profile pages, domains, hosts, and delivery locations to change.

## Disclosure, verification, and rights remain separate

- A public display name does not make every claim public.
- A disclosed claim is not necessarily verified.
- A verified proposition does not require revealing its underlying personal value.
- A public media reference does not prove ownership or redistribution rights.
- A licensing declaration is not a completed transaction or a legal authorization.

Profiles can describe claims and disclosure intent; the consuming host remains responsible for authentication, authorization enforcement, trusted verification, legal process, and secure handling.

## Dependency policy

The portable CreatorNetwork wire contracts should remain small, versioned, and straightforward to exchange. They need not expose Profiles' entire object model or force every independent reader to load the Profiles package just to parse a publication. Instead, provide a deliberate adapter at the authoring/host boundary and test it against the real Profiles API.

This is interoperability, not duplication: Profiles remains authoritative for profile semantics; CreatorNetwork remains authoritative for publication semantics. If a later contract truly requires a shared Profiles type, introduce that dependency only with a demonstrated compatibility and performance benefit.

## Acceptance checks

- [ ] A creator can publish under a persona without exposing the canonical profile or private identity links.
- [ ] Only explicitly disclosed profile fields enter a public publication.
- [ ] A host cannot treat CreatorIdentity.DisplayName or ProfileUri as authentication proof.
- [ ] Missing, unknown, or untrusted identity mappings fail closed for protected publishing operations.
- [ ] Public manifest reads do not return raw Profile objects, private claims, or access records.
- [ ] Profile disclosure tests and CreatorNetwork serialization tests cover the adapter boundary.
- [ ] Export/import preserves stable identity references without treating old host URLs as identity.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
