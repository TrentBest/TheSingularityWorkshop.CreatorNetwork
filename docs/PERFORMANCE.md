![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Performance: measure the whole publication path

CreatorNetwork prioritizes throughput, low allocation, and predictable tail latency where those properties improve creator and viewer experience. Complexity is acceptable when measurement shows a material win and the operational cost is documented.

## Performance rules

- Measure before optimizing, and retain reproducible benchmark cases.
- Benchmark the hot path and the end-to-end path separately. Fast validation does not compensate for slow storage, network, or media delivery.
- Track both elapsed time and allocated bytes; add throughput and tail-latency measurements when the implementation supports meaningful load tests.
- Keep benchmark names stable and specific so historical results can be compared.
- Record runtime, OS, CPU, configuration, workload size, and commit alongside results.
- Treat unexplained regressions as defects to investigate. Do not conceal regressions by changing the workload or dropping historical data.
- Prefer designs that move work to publication creation/import rather than repeating it for every read, provided the data remains safe and correct.
- Avoid speculative complexity: a faster microbenchmark is not sufficient if it worsens the end-to-end workload.

## Current baseline

No measurements have been recorded yet. The benchmark project establishes repeatable validation, JSON serialization/deserialization, and warm file-store read workloads; its first verified run should become the baseline. Never present unexecuted benchmark code as measured performance.

## Run benchmarks

From the repository root:

```bash
dotnet run -c Release --project benchmarks/CreatorNetwork.Benchmarks -- --filter '*'
```

BenchmarkDotNet writes detailed results under `BenchmarkDotNet.Artifacts`. Preserve the raw reports when establishing a baseline and compare like-for-like hardware/runtime configurations.

## Benchmark scope to grow

1. Contract validation by manifest size and error count (initial cases implemented).
2. JSON parse/write throughput and allocated bytes (small and 100-reference manifests implemented).
3. Warm cached file-store reads (initial case implemented); cold disk reads and write durability remain to be measured.
4. File-backed publish/export/import and concurrent readers/updates.
5. Media inventory hashing and integrity checks.
6. HTTP serving, range requests, caching, and concurrency.
7. End-to-end publish → discover → consume → migrate.

A benchmark result must identify exactly what was measured. Metadata throughput and media streaming throughput are different metrics.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
