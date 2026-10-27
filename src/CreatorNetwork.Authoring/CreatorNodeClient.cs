using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CreatorNetwork.Contracts;

namespace CreatorNetwork.Authoring;

/// <summary>
/// Host-neutral client for the recorded-media slice of a Creator Node.
/// Capture and permission prompts belong to the host adapter; this client accepts
/// an already-recorded stream and never starts recording implicitly.
/// </summary>
public sealed class CreatorNodeClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly Uri _baseUri;

    public CreatorNodeClient(HttpClient httpClient, Uri baseUri)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _baseUri = ValidateBaseUri(baseUri);
    }

    /// <summary>
    /// Uploads finalized recorded bytes. The caller must obtain any required capture
    /// permission and obtain explicit user confirmation before invoking publication.
    /// </summary>
    public async Task<MediaAssetDescriptor> UploadAsync(
        string assetId,
        string mediaType,
        Func<CancellationToken, ValueTask<Stream>> openReadStream,
        long? lengthBytes = null,
        string? sha256 = null,
        CancellationToken cancellationToken = default)
    {
        ValidateAssetId(assetId);
        if (string.IsNullOrWhiteSpace(mediaType) || !MediaTypeHeaderValue.TryParse(mediaType, out _))
            throw new ArgumentException("A valid media type is required.", nameof(mediaType));
        ArgumentNullException.ThrowIfNull(openReadStream);
        if (lengthBytes is < 0)
            throw new ArgumentOutOfRangeException(nameof(lengthBytes));

        await using Stream stream = await openReadStream(cancellationToken).ConfigureAwait(false);
        using var content = new StreamContent(stream);
        content.Headers.ContentType = MediaTypeHeaderValue.Parse(mediaType);
        if (lengthBytes.HasValue)
            content.Headers.ContentLength = lengthBytes.Value;

        using var request = new HttpRequestMessage(HttpMethod.Put, AssetUri(assetId)) { Content = content };
        if (!string.IsNullOrWhiteSpace(sha256))
            request.Headers.TryAddWithoutValidation("X-Content-SHA256", sha256);

        using HttpResponseMessage response = await _httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);

        if (response.StatusCode == HttpStatusCode.Conflict)
            throw new InvalidOperationException($"Asset '{assetId}' already exists. Choose a new stable asset ID; existing assets are not overwritten.");
        response.EnsureSuccessStatusCode();

        MediaAssetDescriptor? descriptor = await response.Content.ReadFromJsonAsync<MediaAssetDescriptor>(
            JsonOptions, cancellationToken).ConfigureAwait(false);
        if (descriptor is null ||
            !string.Equals(descriptor.AssetId, assetId, StringComparison.Ordinal) ||
            !string.Equals(descriptor.MediaType, mediaType, StringComparison.OrdinalIgnoreCase) ||
            descriptor.LengthBytes < 0 ||
            !IsSha256(descriptor.Sha256))
            throw new InvalidDataException("The Creator Node returned an invalid finalized media descriptor.");

        return descriptor;
    }

    /// <summary>
    /// Downloads an asset from this Node and verifies the received byte count and SHA-256
    /// against a previously trusted finalized descriptor. Bytes are streamed; the whole
    /// asset is never buffered in memory. If verification fails, the destination may already
    /// contain partial or untrusted bytes and should be discarded by the caller.
    /// </summary>
    public async Task DownloadVerifiedAsync(
        MediaAssetDescriptor expected,
        Stream destination,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(expected);
        ArgumentNullException.ThrowIfNull(destination);
        ValidateAssetId(expected.AssetId);
        if (!destination.CanWrite)
            throw new ArgumentException("The destination stream must be writable.", nameof(destination));
        if (expected.LengthBytes < 0 || !IsSha256(expected.Sha256))
            throw new ArgumentException("A valid finalized descriptor with a byte length and SHA-256 is required.", nameof(expected));

        using var request = new HttpRequestMessage(HttpMethod.Get, AssetUri(expected.AssetId));
        using HttpResponseMessage response = await _httpClient.SendAsync(
            request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        string? responseType = response.Content.Headers.ContentType?.MediaType;
        if (responseType is not null &&
            !string.Equals(responseType, expected.MediaType, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException($"The Node returned media type '{responseType}', expected '{expected.MediaType}'.");

        string? etag = response.Headers.ETag?.Tag;
        if (!string.IsNullOrWhiteSpace(etag) &&
            etag.StartsWith("\"sha256-", StringComparison.Ordinal) && etag.EndsWith("\"", StringComparison.Ordinal))
        {
            string etagDigest = etag[8..^1];
            if (!string.Equals(etagDigest, expected.Sha256, StringComparison.OrdinalIgnoreCase))
                throw new InvalidDataException("The Node's ETag digest does not match the expected asset descriptor.");
        }

        await using Stream source = await response.Content.ReadAsStreamAsync(cancellationToken).ConfigureAwait(false);
        using var hash = System.Security.Cryptography.IncrementalHash.CreateHash(
            System.Security.Cryptography.HashAlgorithmName.SHA256);
        byte[] buffer = new byte[64 * 1024];
        long totalBytes = 0;
        while (true)
        {
            int read = await source.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken).ConfigureAwait(false);
            if (read == 0)
                break;

            totalBytes = checked(totalBytes + read);
            hash.AppendData(buffer, 0, read);
            await destination.WriteAsync(buffer.AsMemory(0, read), cancellationToken).ConfigureAwait(false);
        }

        string actualDigest = Convert.ToHexString(hash.GetHashAndReset()).ToLowerInvariant();
        if (totalBytes != expected.LengthBytes)
            throw new InvalidDataException($"Downloaded {totalBytes} bytes, expected {expected.LengthBytes}.");
        if (!string.Equals(actualDigest, expected.Sha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Downloaded media SHA-256 does not match the expected asset descriptor.");
    }

    /// <summary>Publishes a manifest only when the caller explicitly invokes this method.</summary>
    public async Task PublishAsync(ChannelPublication publication, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(publication);
        IReadOnlyList<PublicationValidationError> errors = PublicationValidator.Validate(publication);
        if (errors.Count != 0)
            throw new ArgumentException("The publication failed validation: " +
                string.Join("; ", errors.Select(error => error.Message)), nameof(publication));

        using HttpResponseMessage response = await _httpClient.PutAsJsonAsync(
            new Uri(_baseUri, $"v1/channels/{Uri.EscapeDataString(publication.PublicationId)}"),
            publication, JsonOptions, cancellationToken).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
    }

    /// <summary>Creates a portable content reference from a finalized upload descriptor.</summary>
    public ContentReference CreateContentReference(MediaAssetDescriptor descriptor)
    {
        ArgumentNullException.ThrowIfNull(descriptor);
        ValidateAssetId(descriptor.AssetId);
        return new ContentReference(
            descriptor.AssetId,
            descriptor.MediaType,
            descriptor.LengthBytes,
            descriptor.Sha256,
            AssetUri(descriptor.AssetId).AbsoluteUri);
    }

    private Uri AssetUri(string assetId) =>
        new(_baseUri, $"v1/assets/{Uri.EscapeDataString(assetId)}");

    private static Uri ValidateBaseUri(Uri baseUri)
    {
        ArgumentNullException.ThrowIfNull(baseUri);
        if (!baseUri.IsAbsoluteUri || (baseUri.Scheme != Uri.UriSchemeHttp && baseUri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException("The Creator Node base URI must be an absolute HTTP(S) URI.", nameof(baseUri));

        // The current Node has no authentication. Refuse accidental remote plaintext use.
        bool loopback = baseUri.IsLoopback || string.Equals(baseUri.Host, "localhost", StringComparison.OrdinalIgnoreCase);
        if (baseUri.Scheme != Uri.UriSchemeHttps && !loopback)
            throw new ArgumentException("The unauthenticated development Node may only be used over loopback HTTP. Use HTTPS for a remote host.", nameof(baseUri));

        return baseUri.AbsoluteUri.EndsWith("/", StringComparison.Ordinal)
            ? baseUri
            : new Uri(baseUri.AbsoluteUri + "/", UriKind.Absolute);
    }

    private static void ValidateAssetId(string assetId)
    {
        if (string.IsNullOrWhiteSpace(assetId) || assetId.Length > 200 ||
            assetId.Any(char.IsControl) || assetId is "." or "..")
            throw new ArgumentException("Asset ID must be a non-empty opaque identifier of at most 200 characters.", nameof(assetId));
    }

    private static bool IsSha256(string value) =>
        value.Length == 64 && value.All(Uri.IsHexDigit);
}
