![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Development Guide

CreatorNetwork starts with contracts and a proving slice. Keep the portable contract layer independent of provider-specific SDKs, and keep end-to-end behavior testable without cloud credentials.

## Working principles

- Keep the portable contract layer free of provider-specific SDKs.
- Define stable identifiers and versioning before building multiple services around them.
- Keep identity, publication, storage, discovery, media delivery, and presentation separate.
- Prefer existing protocols after a documented fit assessment over inventing a replacement.
- Keep examples runnable without cloud credentials.
- Test export/import and cross-origin consumption as first-class behavior.
- Test media portability separately from metadata portability.
- Do not claim a capability is implemented until code and tests demonstrate it.
- Treat performance as a product requirement: measure throughput, allocations, and end-to-end latency on stable workloads.
- Accept extra complexity when repeatable measurements demonstrate a material gain and the operational trade-off is documented.

## Current implementation

- `src/CreatorNetwork.Contracts`: provider-neutral .NET Standard 2.0 models and validation.
- `src/CreatorNetwork.Node`: .NET 8 loopback-only prototype with file-backed publication metadata, bounded streaming media ingest, SHA-256 verification, descriptor persistence, and range-enabled asset delivery. It remains unauthenticated and development-only.
- `src/CreatorNetwork.Profiles`: .NET 8 source-integrated publication adapter; requires a local Profiles development checkout. It checks the authenticated-principal flag, validates the audience ID, and delegates trusted persona resolution/disclosure to a host-owned resolver. It is not itself an authentication provider or authorization enforcement layer.
- `tests/CreatorNetwork.Contracts.Tests`: contract validation, publication file-store persistence, and media asset ingest/integrity/storage tests. The new media tests have not yet been executed.
- `tests/CreatorNetwork.Profiles.Tests`: focused projection and publication-boundary tests, pending execution against the dependency source checkout.
- `benchmarks/CreatorNetwork.Benchmarks`: named BenchmarkDotNet validation workloads.

The Node is a development prototype. It has no authentication or production privacy enforcement and must not be exposed to a network.

## Build and test

With the .NET 8 SDK, run the contract tests:

```powershell
dotnet test .\tests\CreatorNetwork.Contracts.Tests\CreatorNetwork.Contracts.Tests.csproj -c Release
```

To test the Profiles adapter, check out `TheSingularityWorkshop.Profiles` on its `development` branch beside this repository and pass the absolute path to its project:

```powershell
$profilesProject = (Resolve-Path ..\TheSingularityWorkshop.Profiles\TheSingularityWorkshop.Profiles.csproj).Path
dotnet test .\tests\CreatorNetwork.Profiles.Tests\CreatorNetwork.Profiles.Tests.csproj -c Release -p:ProfilesProjectPath="$profilesProject"
```

CI must record the exact dependency commit SHA alongside the test evidence. Source-integrated validation does not prove that a downstream package can restore published NuGet dependencies. CiAi orchestration is tracked in [CiAi issue #13](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/13); until the supported integration is in place, do not claim a green CiAi-driven run.

To run the prototype and benchmarks, see [Local Media Assets](MEDIA_ASSETS.md) for upload/download examples. The media store is loopback-only and must not be exposed to a network.

To run the prototype and benchmarks:

```powershell
dotnet run -c Release --project src/CreatorNetwork.Node
dotnet run -c Release --project benchmarks/CreatorNetwork.Benchmarks -- --filter '*'
```

Benchmark details and rules are in [Performance](PERFORMANCE.md). A numeric baseline must not be claimed until the first benchmark run is executed and preserved.

## Suggested implementation order

1. Contract models and validation.
2. A local/file-backed Creator Node and bounded recorded-media asset store (implementation added; test execution still required).
3. Profiles host adapter and concrete trusted publisher/persona resolver backed by the host's identity-link store.
4. An independent publisher and consumer.
5. Export/import with a media inventory.
6. Cross-origin integration tests.
7. Protocol interoperability proof.
8. Security and operational hardening.

The contract models, local Node prototype, and a narrow identity projection adapter exist. The adapter does not yet implement trusted publisher resolution or prove the full host-to-publication privacy boundary. Independent publisher/consumer programs and export/import remain future proving work.

## Branch and release discipline

Use `development` for active work and `master` as the release reference. Keep the repository to those two long-lived branches. Do not publish packages or create a public release unless explicitly authorized by the maintainer.

## CI and CiAi

Continuous integration should use The Singularity Workshop’s CiAi rather than quietly creating a parallel orchestration system. The onboarding gap is tracked in [CiAi issue #13](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/13). Until the supported integration is confirmed, do not add a misleading green workflow that claims unexecuted build, test, or documentation checks passed.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
