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
- `src/CreatorNetwork.Node`: .NET 8 loopback-only file-backed prototype with in-memory response caching and conditional ETags.
- `tests/CreatorNetwork.Contracts.Tests`: contract validation and file-store persistence tests.
- `benchmarks/CreatorNetwork.Benchmarks`: named BenchmarkDotNet validation workloads.

The Node is a development prototype. It has no authentication or production privacy enforcement and must not be exposed to a network.

## Build and test

With the .NET 8 SDK:

```bash
dotnet test tests/CreatorNetwork.Contracts.Tests/CreatorNetwork.Contracts.Tests.csproj -c Release
dotnet run -c Release --project src/CreatorNetwork.Node
dotnet run -c Release --project benchmarks/CreatorNetwork.Benchmarks -- --filter '*PublicationValidationBenchmarks*'
```

Benchmark details and rules are in [Performance](PERFORMANCE.md). The first benchmark run must be executed and preserved before a numeric baseline is claimed.

## Suggested implementation order

1. Contract models and validation.
2. A local/file-backed Creator Node.
3. An independent publisher and consumer.
4. Export/import with a media inventory.
5. Cross-origin integration tests.
6. Protocol interoperability proof.
7. Security and operational hardening.

The first two items have prototypes; the remaining items are the next proving work. This sequence does not require a separate package or service for each concern.

## Branch and release discipline

Use `development` for active work and `master` as the release reference. Keep the repository to those two long-lived branches. Do not publish packages or create a public release unless explicitly authorized by the maintainer.

## CI and CiAi

Continuous integration should use The Singularity Workshop’s CiAi rather than quietly creating a parallel orchestration system. The onboarding gap is tracked in [CiAi issue #13](https://github.com/TrentBest/TheSingularityWorkshop.CiAi/issues/13). Until the supported integration is confirmed, do not add a misleading green workflow that claims unexecuted build, test, or documentation checks passed.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
