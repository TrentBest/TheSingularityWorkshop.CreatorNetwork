![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Getting Started: the first vertical slice

The first milestone is intended to prove creator portability end to end. The current runnable slice demonstrates local contract validation, metadata storage, and reads; it does not yet prove complete portability. CreatorNetwork is the creator-facing Content Creator platform, but this repository's current runnable Node is only one local proving component—not the finished creator workspace or production network.

> **Current status:** v1 portable channel contracts, additive v2 channel/publication models, validation tests, a runnable museum representation contract sample, and a loopback-only file-backed Creator Node prototype exist. The v2 contracts are not yet wired into the Node. Independent provider-backed publisher/consumer applications and export/import remain in progress.

## 1. Run the tests

With the .NET 10 SDK installed, from the repository root:

```bash
dotnet test tests/CreatorNetwork.Contracts.Tests/CreatorNetwork.Contracts.Tests.csproj -c Release
```

The repository CI also runs the Profiles integration tests, museum consumer sample, and benchmark build. See the [latest verified CI run](https://github.com/TrentBest/TheSingularityWorkshop.CreatorNetwork/actions/runs/45755130931).

## 2. Start the local Creator Node

```bash
dotnet run -c Release --project src/CreatorNetwork.Node
```

The service binds to `127.0.0.1:5080` only. Its default data directory is `.creatornetwork-data` in the current working directory. Set `CREATORNETWORK_DATA` to choose another local directory.

Check health:

```bash
curl http://127.0.0.1:5080/health
```

## 3. Publish a v1 channel manifest

The current local Node accepts the existing `creator-network.channel/1` shape. The additive v2 `CreatorChannel` and `PublicationSnapshot` contracts are validated in the contracts project but are not yet connected to Node persistence or HTTP routes.

In a second terminal, save this sample as `channel.json`:

```json
{
  "schema": "creator-network.channel/1",
  "publicationId": "demo-channel",
  "creator": {
    "id": "creator:demo",
    "displayName": "Demo Creator",
    "profileUri": null
  },
  "title": "My first portable channel",
  "description": "A local CreatorNetwork proof.",
  "visibility": "public",
  "revision": 1,
  "updatedAt": "2026-10-26T00:00:00Z",
  "content": [
    {
      "id": "content:sample-video",
      "mediaType": "video/mp4",
      "lengthBytes": 0,
      "sha256": null,
      "sourceUri": "https://example.test/sample.mp4"
    }
  ],
  "attribution": "Replace with the actual asset attribution."
}
```

Publish it to the local node:

```bash
curl -i -X PUT http://127.0.0.1:5080/v1/channels/demo-channel \
  -H "Content-Type: application/json" \
  --data-binary @channel.json
```

A successful write returns `204 No Content`. The media URL is an example reference, not a real asset and not a claim that the media has been downloaded or verified.

## 4. Read the publication

```bash
curl -i http://127.0.0.1:5080/v1/channels/demo-channel
```

The response includes an ETag. Send it back with `If-None-Match` to test conditional reads; an unchanged manifest returns `304 Not Modified`. The node caches validated manifest objects, serialized response bytes, and ETags in memory to avoid repeated disk reads and JSON serialization on the hot path.

## What this prototype does—and does not—guarantee

- It validates the v1 schema, identity references, revision, visibility declaration, timestamp, and content metadata.
- It stores v1 metadata locally and atomically replaces a manifest file.
- It serves media bytes only for assets explicitly uploaded to the local `/v1/assets/{assetId}` endpoint; it does not automatically fetch `ContentReference.SourceUri` locations or prove cross-provider media migration.
- It does **not** enforce private/unlisted authorization, prove creator ownership, sign manifests, or provide a production authentication boundary.
- It is bound to loopback and has no authentication. **Do not expose it to a network or use it for real private content.**
- The v2 contract models do not yet have a persistence or HTTP implementation. The museum sample demonstrates provenance/rights-aware contract selection in memory, not remote provider delivery.
- Metadata portability is not media-byte portability. Export/import and independent-client acceptance tests remain required.

## End-to-end acceptance criteria

- Publisher and consumer are separate programs or processes.
- Publication uses a documented, versioned contract.
- Consumer works without a hard-coded creator/channel record.
- Export/import preserves identity references and publication versions.
- Media portability is tested separately from metadata portability.
- Missing media and unsupported manifest versions produce actionable errors.
- At least two origins can serve the same publication contract.
- A showcase Experience consumes the publication through the contract rather than embedding content directly.
- Unauthorized metadata and byte reads fail closed at the trusted host/provider boundary.

## What comes after the slice

The next implementation step is a provider-backed museum publication flow: publish a v2 snapshot that references an immutable Experience artifact and typed supporting resources, then resolve it from a separate consumer while preserving the current active Experience on failure. The local Experience artifact/catalog provider gap is tracked upstream in [MicroBundleRepository issue #22](https://github.com/TrentBest/TheSingularityWorkshop.MicroBundleRepository/issues/22); CreatorNetwork must reuse that provider boundary rather than create a competing artifact store.

Later work can add signed publications, explicit sharing-policy enforcement, multiple discovery providers, subscription export, media replication, and improved streaming. Each addition needs a concrete consumer and a testable contract.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
