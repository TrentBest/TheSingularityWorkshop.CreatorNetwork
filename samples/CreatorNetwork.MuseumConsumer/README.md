# Museum representation consumer sample

Run from the repository root:

```sh
dotnet run --project samples/CreatorNetwork.MuseumConsumer/CreatorNetwork.MuseumConsumer.csproj --configuration Release
```

The sample constructs an in-memory museum source artifact, an approved low-complexity alternative, separate rights declarations, and a source-publisher representation offer. It validates the provenance and cross-references, then demonstrates a consumer requesting `gallery.centerpiece` with `Embed` rights. The original grants only `Display`; the alternate grants `Display` and `Embed`.

The consumer selects the exact source-approved alternative and never fetches or transforms the restricted original.

**This is a runnable contract demonstration, not a production consumer or rights engine.** The publisher/rights-holder allow-list is hard-coded to stand in for a trusted host's real identity and authority checks. There is no HTTP transport, remote storage, legal license verification, persistence, or cache/takedown propagation in this sample.
