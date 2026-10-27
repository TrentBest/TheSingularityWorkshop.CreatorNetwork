![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# Creator Studio, Capture, and Streaming Boundaries

CreatorNetwork is the publishing and portability layer—not the editor, capture engine, transcoder, or player. A creator must be able to make content, publish it, and reach viewers without making one application or hosting company the owner of the whole workflow.

This boundary keeps the network useful to independent tools while still making creator authoring a first-class ecosystem goal.

## The user-facing workflow

A creator-facing experience should make the following path possible:

1. **Create or capture.** Record audio/video, capture a screen or other supported source, import existing media, or author structured content.
2. **Review and prepare.** Preview the result, inspect metadata, choose a thumbnail or representation, and confirm attribution, rights, and intended audience.
3. **Publish.** Create or update a versioned CreatorNetwork publication that references the media and its approved metadata.
4. **Deliver.** Store or stream media through a replaceable content provider; do not embed large media bytes in the publication manifest.
5. **Discover.** Make the publication discoverable through one or more independently operated discovery providers.
6. **Consume.** Present it through a web client, desktop application, AnyApp, or an Experience.
7. **Move.** Export publication metadata and a media inventory, then verify that another host can import or serve the work without changing the creator's portable identity.

A local prototype is useful for proving the workflow, but it is not a production service merely because it can write a manifest to disk.

## Responsibility boundaries

| Capability | Owner / boundary | Rule |
|---|---|---|
| Creator-facing editor and workflow | A creator-facing application or Experience | UI and workflow may evolve independently of the portable protocol. |
| Microphone, screen, device, and other capture | Host-specific capture adapters such as UserIO | Ask only for the source the creator selected; report permission, availability, and failures honestly. |
| Media editing, encoding, thumbnails, and transcoding | Authoring/media tools or replaceable media providers | Keep codecs and heavyweight media dependencies out of the portable contracts. |
| Portable creator identity and publication metadata | CreatorNetwork | Publish only approved identity projections; a display name or profile URI is not authentication proof. |
| Media bytes, upload, byte ranges, live delivery, and replication | Content/media provider boundary | Metadata references media; they do not imply the bytes are stored, reachable, or portable. |
| Search, feeds, recommendations, and indexes | Discovery provider boundary | No mandatory global feed or single ranking authority. |
| Playback and presentation | Independent clients / Experiences | Consumers choose their own UI and may render the same publication differently. |
| Identity semantics and audience-specific disclosure | Profiles plus trusted host integration | Do not serialize a complete Profile into a public publication. |
| Runtime capability composition | FSM_COS, when a host needs it | Composition does not turn FSM_COS into the creator registry, media host, or recommendation authority. |

**Placement decision:** keep the portable publishing protocol in CreatorNetwork. Put creator UI and workflows in an application/Experience, reusing shared capture and media components. Do not move the creator studio wholesale into the CreatorNetwork contract library, and do not make Experiences the canonical storage or publication authority.

## Recorded media and live streaming are different lifecycles

Recorded media usually has a durable asset identity, known or finalized length, content type, integrity metadata, and one or more delivery locations. It can be uploaded, checked, replicated, and referenced by a versioned publication.

A live stream is time-bound and operational. It needs a stream/session identity, lifecycle state, start/end information, an authorized publishing path, viewer delivery endpoints, reconnect behavior, and an explicit policy for whether the live session is later retained as a recording. A manifest pointing at a live endpoint does not guarantee the stream is active or that the viewer is authorized.

Keep the portable concepts separate:

- **Publication:** stable, versioned statement of what the creator publishes.
- **Media asset:** durable or finalized content with integrity and delivery references.
- **Live session:** transient session metadata and lifecycle state.
- **Delivery endpoint:** provider-specific location/protocol, resolved at the edge rather than baked into creator identity.
- **Recording relationship:** optional link from a completed live session to a durable asset.

The first proving slice should prefer ordinary HTTPS media delivery and a simple recorded asset. Add live streaming as a distinct capability only when a concrete publisher and consumer can test session creation, authorization, viewer delivery, interruption/reconnect, finalization, and optional recording. Do not pretend that an HTML capture prototype alone implements network streaming.

## Security and creator control

- Capture requires explicit source selection and clear feedback about what is being captured.
- Publication visibility is a declaration, not authorization enforcement. The serving host must enforce access at metadata, asset, and stream endpoints.
- A creator's right to upload or stream an asset must not be inferred from possession of its bytes.
- Never expose private canonical identity links when publishing under a persona.
- Do not silently turn recording, audience analytics, advertising, or monetization on because a creator publishes.
- Preserve a self-hosted and non-monetized path.
- Avoid provider-specific credentials and URLs in the portable creator identity.
- Treat live endpoints and upload credentials as secrets; do not place short-lived credentials in durable public manifests.
- Export should describe both metadata and media inventory, and clearly report assets that could not be copied.

## Delivery roadmap

### First: prove recorded publication portability

- [ ] Capture or import one small recorded asset through a creator-facing tool.
- [ ] Preview and explicitly confirm the asset and publication metadata.
- [ ] Upload/store the asset through a replaceable media provider.
- [ ] Publish a versioned manifest with stable asset identity, media type, size when known, integrity digest when available, attribution, rights declaration, and delivery reference.
- [ ] Consume the publication in a separate client without hard-coding the channel.
- [ ] Export the manifest and media inventory; import to a second local/provider origin.
- [ ] Verify content bytes and creator/publication identity independently.
- [ ] Make missing media and rejected uploads visible and recoverable.

### Next: make media delivery robust

- [ ] Support large uploads, cancellation, retries, resumability where the provider supports it, and bounded memory use.
- [ ] Test byte-range delivery and client seeking for supported recorded formats.
- [ ] Define cache and replication behavior without coupling the portable contract to one cloud vendor.
- [ ] Add explicit content access authorization and abuse/reporting controls before network exposure.

### Then: define live-session interoperability

- [ ] Define a versioned live-session descriptor separately from durable publication metadata.
- [ ] Specify publish authorization, viewer authorization, expiry, endpoint discovery, and session lifecycle.
- [ ] Test disconnect/reconnect, stream end, endpoint failure, and the transition from live session to optional recording.
- [ ] Demonstrate at least two independent implementations or a provider swap.
- [ ] Document protocol/codec support and real operational limits; do not claim universal streaming compatibility.

## What this boundary does not claim

The current loopback-only Creator Node is a metadata prototype. It does not serve media bytes, implement a production upload pipeline, provide authenticated multi-user hosting, enforce private publication policy, or implement live streaming. These are explicit next capabilities, not hidden guarantees.

The measure of success is creator choice: content can be created with one tool, published through one host, discovered by another service, consumed in another client, and moved to another provider without surrendering portable identity or publication history.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
