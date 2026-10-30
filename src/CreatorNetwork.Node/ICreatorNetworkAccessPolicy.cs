using CreatorNetwork.Contracts;
using System.Net;

namespace CreatorNetwork.Node;

/// <summary>
/// Host-owned authorization boundary for publication metadata and artifact bytes.
/// Implementations must authenticate and authorize each operation; a Profiles projection
/// or publication visibility value is not an authorization grant.
/// </summary>
public interface ICreatorNetworkAccessPolicy
{
    ValueTask<bool> CanReadPublicationAsync(
        HttpContext context, string publicationId, CancellationToken cancellationToken);

    ValueTask<bool> CanWritePublicationAsync(
        HttpContext context, ChannelPublication publication, CancellationToken cancellationToken);

    ValueTask<bool> CanReadAssetAsync(
        HttpContext context, string assetId, CancellationToken cancellationToken);

    ValueTask<bool> CanWriteAssetAsync(
        HttpContext context, string assetId, CancellationToken cancellationToken);
}

/// <summary>
/// Explicitly development-only policy for the loopback file-backed node.
/// It grants no identity-based or private-audience guarantee and must be replaced by a
/// trusted host policy before any network exposure or use with private data.
/// </summary>
public sealed class LoopbackDevelopmentAccessPolicy : ICreatorNetworkAccessPolicy
{
    public ValueTask<bool> CanReadPublicationAsync(
        HttpContext context, string publicationId, CancellationToken cancellationToken) =>
        ValueTask.FromResult(IsLoopback(context));

    public ValueTask<bool> CanWritePublicationAsync(
        HttpContext context, ChannelPublication publication, CancellationToken cancellationToken) =>
        ValueTask.FromResult(IsLoopback(context));

    public ValueTask<bool> CanReadAssetAsync(
        HttpContext context, string assetId, CancellationToken cancellationToken) =>
        ValueTask.FromResult(IsLoopback(context));

    public ValueTask<bool> CanWriteAssetAsync(
        HttpContext context, string assetId, CancellationToken cancellationToken) =>
        ValueTask.FromResult(IsLoopback(context));

    private static bool IsLoopback(HttpContext context) =>
        context.Connection.RemoteIpAddress is { } address && IPAddress.IsLoopback(address);
}
