![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# HTTP and REST Ownership Map

This document records the current source-grounded transport boundary for CreatorNetwork. It is an implementation inventory, not a claim that the complete network or cross-provider workflow is verified.

## Decision

- **FSM_REST** owns reusable REST capability descriptors and transport contracts where its current API fits.
- **CreatorNetwork** owns publication and media domain contracts, endpoint paths, validation, integrity rules, and publication workflow.
- **ASP.NET Core / the host** owns incoming HTTP routing, request-body lifecycle, response writing, and platform-specific behavior in the local Creator Node.
- **Raw media bytes are not REST text bodies.** Keep media ingest and delivery on streaming-capable HTTP paths; do not buffer media or convert it to a string merely to route it through a generic REST abstraction.
- **HttpClient is an implementation detail, not a second REST framework.** Reuse a host-configured client and avoid adding another generic client abstraction.

## Current call-site inventory

| Source path / operation | Current implementation | Classification | Required action |
|---|---|---|---|
| `src/CreatorNetwork.Node/Program.cs` — `GET/PUT /v1/channels/{publicationId}` | ASP.NET Core minimal API routes; JSON publication DTO; file-backed store | Host-owned REST endpoint and domain-specific publication API | Keep route, validation, ETag, persistence, and publication semantics here. The host owns incoming transport mechanics. |
| `src/CreatorNetwork.Node/Program.cs` — `PUT/GET /v1/assets/{assetId}` | ASP.NET Core request-body stream and `Results.Stream` with range processing | Binary/media transport | Keep streaming, cancellation, integrity, bounded size, and byte-range behavior on stream-capable APIs. Do not pass media through text-only REST bodies. |
| `src/CreatorNetwork.Authoring/CreatorNodeClient.cs` — `UploadAsync` | `HttpRequestMessage`, `StreamContent`, `ResponseHeadersRead`; reads the small finalized JSON descriptor | Binary upload plus JSON control response | Retain streaming-capable transport. Tests must cover cancellation, non-success responses, and bounded-memory behavior. |
| `src/CreatorNetwork.Authoring/CreatorNodeClient.cs` — `DownloadVerifiedAsync` | `HttpRequestMessage`, `ResponseHeadersRead`, stream copy with incremental SHA-256 and length checks | Binary/media delivery | Retain streaming-capable transport. Verify cancellation, HTTP errors, type/length/digest mismatches, and that failed output is not treated as verified. |
| `src/CreatorNetwork.Authoring/CreatorNodeClient.cs` — `PublishAsync` | `HttpClient.PutAsJsonAsync` sends a validated `ChannelPublication` and requires success | JSON REST/control-plane operation | This is the clear FSM_REST integration candidate. The current Authoring project has no FSM_REST project/package reference, so do not claim it already uses FSM_REST. Integrate only through the verified API and the repository's source-integrated dependency workflow; keep the public method's explicit publication boundary. |
| `src/CreatorNetwork.Authoring/CreatorNodeClient.cs` — base URI validation | Rejects non-loopback plaintext HTTP because the prototype has no authentication | Client-side safety guard | Keep local security policy here; HTTPS alone does not make the unauthenticated Node production-ready. |

## Verified FSM_REST fit and limit

The FSM_REST development source currently exposes `TheSingularityWorkshop.FSM_REST.IRestTransport`, `RestRequest`, `RestResponse`, and `HttpClientRestTransport`. The concrete HTTP adapter sends with `ResponseHeadersRead`, but it constructs a `StringContent` from `RestRequest.Body` and reads the response body as a string. Therefore:

- A small JSON manifest operation can fit this transport after its status/header/body semantics and source-integrated dependency are tested.
- The current text-body contract is not appropriate for CreatorNetwork's media upload/download path.
- A successful transport response is not proof that the publication is authorized or that an asset is accessible. CreatorNetwork and the serving host retain domain validation and authorization responsibilities.
- The current source has not yet been integrated with FSM_REST and no build/test result is claimed here.

## Required implementation and verification

1. Add the source-integrated FSM_REST reference through the established development workflow, pin and report the exact dependency commit, and keep package-consumer validation separate.
2. Route the JSON publication operation through the verified FSM_REST transport without moving CreatorNetwork validation or publication policy into FSM_REST.
3. Keep asset upload/download on streaming-capable APIs; do not add a new generic HTTP-wrapper package.
4. Add deterministic tests for publication status/error propagation, cancellation, asset upload cancellation, download cancellation, byte-length and SHA-256 mismatches, and non-success responses. Use fake handlers/transports; no live service or credentials.
5. Execute the test suite and record exact source/dependency SHAs before marking this boundary complete.

## Security and deployment

The Creator Node is an unauthenticated, loopback-only development prototype. Do not expose it to a network. Its media size limit and integrity checks are not a substitute for authentication, authorization, quotas, abuse controls, or a production threat model.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
