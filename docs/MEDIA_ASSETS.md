![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Local Media Assets

CreatorNetwork now has a small loopback-only proving slice for storing and retrieving recorded media bytes. It is intended to validate the boundary between publication metadata and media delivery—not to serve the public internet.

## What it provides

- `PUT /v1/assets/{assetId}` streams a new asset into local storage using a fixed-size buffer.
- The Node computes SHA-256 while writing and records the finalized byte length, media type, and digest.
- Optional `Content-Length` and `X-Content-SHA256` declarations are checked against the received bytes.
- A configured 512 MiB maximum prevents an unbounded single upload.
- Temporary files are finalized only after the upload completes and declared integrity checks pass.
- Duplicate finalized asset IDs are rejected rather than silently replacing creator content.
- `GET /v1/assets/{assetId}` streams the stored bytes and enables HTTP byte-range responses for clients that seek.
- Asset IDs are mapped to hashed filenames; IDs are not treated as filesystem paths.
- The asset ETag is derived from the finalized content digest.

The endpoint is deliberately simple: it is a development API, not a standardized cross-provider upload protocol.

## Run locally

From the repository root, start the Node:

```powershell
dotnet run -c Release --project .\src\CreatorNetwork.Node
```

The service binds to `http://127.0.0.1:5080`. Keep it on loopback.

Upload a small existing media file:

```powershell
$assetId = "demo:clip-001"
$path = ".\clip.mp4"
$hash = (Get-FileHash $path -Algorithm SHA256).Hash.ToLowerInvariant()
curl.exe -i -X PUT "http://127.0.0.1:5080/v1/assets/$assetId" -H "Content-Type: video/mp4" -H "X-Content-SHA256: $hash" --data-binary "@$path"
```

A successful upload returns `201 Created` and a descriptor containing the stable asset ID, media type, actual byte length, and computed digest. Re-uploading the same ID returns `409 Conflict`.

Download or verify the saved bytes:

```powershell
curl.exe -f "http://127.0.0.1:5080/v1/assets/demo%3Aclip-001" -o .\clip.downloaded.mp4
Get-FileHash .\clip.downloaded.mp4 -Algorithm SHA256
```

Use the digest from the upload response to compare the downloaded bytes. For large files, the server streams through the response rather than loading the complete file into memory.

## Error behavior

| Condition | Response |
|---|---|
| Asset ID is invalid | `400 Bad Request` |
| Declared digest or length does not match received bytes | `400 Bad Request` |
| Asset ID already has a finalized asset | `409 Conflict` |
| Upload exceeds the configured maximum | `413 Payload Too Large` |
| Asset does not exist | `404 Not Found` |
| Digest matches and asset is stored | `201 Created` |

A disconnected or cancelled upload should leave no finalized descriptor. Temporary files are cleaned up in normal exception/cancellation paths. If the process stops between moving the asset and its descriptor, a later upload of the same ID removes the orphaned half-finalized file before retrying.

## Security and lifecycle limits

- There is no authentication or authorization. **Do not expose this Node to a network.**
- The asset store has a global size limit, not per-creator quotas or rate limits.
- Storage is local and does not replicate or export assets to another host.
- The store is append-only by asset ID in this prototype; replacing content requires a new asset ID.
- Media bytes are not automatically linked into a channel manifest. A publisher must deliberately create a `ContentReference` using the returned descriptor and then publish the manifest.
- The service does not capture media, transcode it, create thumbnails, implement resumable uploads, or provide live streaming.
- The current tests are source-level additions and must be executed with the documented .NET test command before this capability is considered verified.

## Test

```powershell
dotnet test .\tests\CreatorNetwork.Contracts.Tests\CreatorNetwork.Contracts.Tests.csproj -c Release
```

See [Creator Studio, Capture, and Streaming Boundaries](CREATOR_STUDIO_AND_STREAMING.md) for the intended product workflow and [Development Guide](DEVELOPMENT.md) for repository guardrails.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
