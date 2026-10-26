![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Getting Started: the first vertical slice

The first milestone proves creator portability end to end. It should not attempt to ship a complete social network, video editor, streaming platform, or recommendation engine.

> **Current status:** portable contracts, validation tests, and a loopback-only file-backed Creator Node prototype exist. Independent publisher/consumer applications and export/import are still in progress. The Node is for local development only, not a production multi-user host.

## 1. Run the tests

With the .NET 8 SDK installed, from the repository root:

```bash
dotnet test tests/CreatorNetwork.Contracts.Tests/CreatorNetwork.Contracts.Tests.csproj -c Release
```

This covers manifest validation and file-store persistence. It does not claim that the full network slice is complete.

## 2. Start the local Creator Node

```bash
dotnet run -c Release --project src/CreatorNetwork.Node
```

The service binds to `127.0.0.1:5080` only. Its default data directory is `.creatornetwork-data` in the current working directory. Set `CREATORNETWORK_DATA` to choose another local directory.

Check health:

```bash
curl http://127.0.0.1:5080/health
```

## 3. Publish a channel manifest

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

- It validates schema, identity references, revision, visibility declaration, timestamp, and content metadata.
- It stores metadata locally and atomically replaces a manifest file.
- It does **not** serve media bytes, enforce private/unlisted authorization, prove creator ownership, sign manifests, or provide a production authentication boundary.
- It is bound to loopback and has no authentication. **Do not expose it to a network or use it for real private content.**
- Metadata portability is not media-byte portability. Export/import and independent-client acceptance tests remain required.

## End-to-end acceptance criteria

- Publisher and consumer are separate programs or processes.
- Publication uses a documented, versioned contract.
- Consumer works without a hard-coded creator/channel record.
- Export/import preserves identity references and publication versions.
- Media portability is tested separately from metadata portability.
- Missing media and unsupported manifest versions produce actionable errors.
- At least two origins can serve the same publication contract.
- A showcase experience consumes the publication through the contract rather than embedding content directly.

## What comes after the slice

After the portability proof works, add signed publications, explicit permission enforcement, multiple discovery providers, subscription export, media replication, and improved streaming. Each addition needs a concrete consumer and a testable contract.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
