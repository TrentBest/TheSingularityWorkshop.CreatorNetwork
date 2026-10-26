![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Development Guide

CreatorNetwork starts with contracts and a proving slice. Keep the first implementation small, portable, testable, and independent of any single host or provider.

## Working principles

- Keep the portable contract layer free of provider-specific SDKs.
- Define stable identifiers and versioning before building multiple services around them.
- Keep identity, publication, storage, discovery, media delivery, and presentation separate.
- Prefer existing protocols after a documented fit assessment over inventing a replacement.
- Keep examples runnable without cloud credentials.
- Test export/import and cross-origin consumption as first-class behavior.
- Test media portability separately from metadata portability.
- Do not claim a capability is implemented until code and tests demonstrate it.

## Suggested implementation order

1. Contract models and validation.
2. A local Creator Node / file-backed publication store.
3. A minimal independent publisher and consumer sample.
4. Export/import with a media inventory.
5. Cross-origin integration tests.
6. Protocol interoperability proof.
7. Security and operational hardening.

This sequence does not require a separate package or service for each step.

## Build and test

Implementation projects and supported SDK versions have not yet been established in this foundation commit. When the first runnable project is added, update this guide and the README with exact commands and verified expected output. Do not publish a package simply because a project builds.

## Branch and release discipline

Use development for active work and master as the release reference. Keep the repository to those two long-lived branches. Do not publish packages or create a public release unless explicitly authorized by the maintainer.

## CI and CiAi

Continuous integration should use The Singularity Workshop’s CiAi rather than quietly creating a parallel orchestration system. If CiAi does not yet support a needed check, record the precise missing capability in a CiAi issue and keep the required check visible. A green workflow must mean the checks it claims to run actually ran; do not disguise unsupported checks as successful ones.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
