![The Singularity Workshop animated moniker](https://raw.githubusercontent.com/TrentBest/TheSingularityWorkshop.Visualize/development/docs/assets/moniker.svg)

# 3. Trust, identity, and rights

CreatorNetwork must not collapse several different questions into one field or boolean.

| Question | What answers it | What does not answer it |
|---|---|---|
| Who is authenticated to this request? | The host's authentication mechanism | A creator ID, display name, or profile URI |
| May this principal publish this record? | Host-enforced authorization and publication policy | A valid DTO or successful schema validation |
| Which persona may be shown to this audience? | Profiles semantics plus a trusted host-provided resolver and disclosure policy | Serializing an entire profile or trusting caller-supplied display fields |
| Are these the expected bytes? | Verification against a trusted expected digest after retrieval | A source URL or a digest copied from untrusted metadata |
| May the creator redistribute content? | Rights evidence and applicable policy/legal review | Attribution text or a hash |
| May a host serve private content? | Access control enforced at the serving boundary | Visibility metadata stored in a manifest |
| Is content safe to execute? | Separate validation, isolation, and host execution policy | Integrity verification or a valid MicroBundle reference |

## Why the Profiles boundary matters

The CreatorNetwork wire contract should carry only the publication-safe identity projection approved for the publication. Profiles owns identity meaning, persona relationships, and observer-specific disclosure. A host adapter must authenticate and authorize independently and use a trusted resolver to obtain the appropriate projection. A profile projection is not itself an authorization grant.

## Why rights and integrity remain separate

A digest can detect whether retrieved bytes match a declared digest. It cannot establish who created those bytes or whether a license permits copying. A rights statement is a claim, not cryptographic proof. Import/export must validate the destination's authority to store and serve content rather than assuming reachability implies permission.

## Local node warning

The domain review identifies the local Creator Node as an unauthenticated, development-only prototype. It must not be treated as production-ready public hosting. Public deployment requires authentication, authorization, quotas and abuse controls, a production threat review, and explicit operational decisions.

## Source evidence and limits

- [Domain and trust review](../DOMAIN_REVIEW.md)
- [Profiles integration](../PROFILES_INTEGRATION.md)
- [Security and trust](../SECURITY_AND_TRUST.md)
- [CreatorIdentity.cs](../../src/CreatorNetwork.Contracts/CreatorIdentity.cs)
- [CreatorNetwork LLM contract](../../CreatorNetworkLLM.md)

These are boundary requirements and documented intent unless a specific implementation and test demonstrates them. Do not infer a security guarantee from architecture prose alone.

---

<!-- THE SINGULARITY WORKSHOP FOOTER -->
<p align="center"><img src="https://raw.githubusercontent.com/TrentBest/FSM_API/master/Documentation/Branding/Avatar.jpg" alt="The Singularity Workshop avatar" width="180" /></p>
