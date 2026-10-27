using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Security.Cryptography;
using CreatorNetwork.Authoring;
using CreatorNetwork.Contracts;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class CreatorNodeClientTests
{
    [Fact]
    public async Task UploadAsync_ReturnsFinalizedDescriptor_WithoutPublishingManifest()
    {
        var descriptor = new MediaAssetDescriptor(
            "recording-001", "audio/webm", 4,
            "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef");
        var handler = new RecordingHandler((request, _) =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/v1/assets/recording-001", request.RequestUri!.AbsoluteUri);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.Created)
            {
                Content = JsonContent.Create(descriptor)
            });
        });
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));
        byte[] bytes = Encoding.UTF8.GetBytes("test");

        MediaAssetDescriptor result = await client.UploadAsync(
            "recording-001", "audio/webm",
            _ => ValueTask.FromResult<Stream>(new MemoryStream(bytes)),
            bytes.Length);

        Assert.Equal(descriptor.AssetId, result.AssetId);
        Assert.Equal(4, result.LengthBytes);
        Assert.Equal(1, handler.RequestCount);
        ContentReference reference = client.CreateContentReference(result);
        Assert.Equal(result.Sha256, reference.Sha256);
        Assert.Equal(result.LengthBytes, reference.LengthBytes);
    }

    [Fact]
    public async Task DownloadVerifiedAsync_StreamsBytesAndVerifiesDigestAndLength()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("recorded media bytes");
        string digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        var expected = new MediaAssetDescriptor("asset-verified", "audio/webm", bytes.Length, digest);
        var handler = new RecordingHandler((request, _) =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.EndsWith("/v1/assets/asset-verified", request.RequestUri!.AbsoluteUri);
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            };
            response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("audio/webm");
            response.Headers.ETag = new System.Net.Http.Headers.EntityTagHeaderValue("\"sha256-" + digest + "\"");
            return Task.FromResult(response);
        });
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));
        await using var destination = new MemoryStream();

        await client.DownloadVerifiedAsync(expected, destination);

        Assert.Equal(bytes, destination.ToArray());
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task DownloadVerifiedAsync_AcceptsPortableContentReferenceWithVerifiedMetadata()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("portable media");
        string digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        var reference = new ContentReference(
            "portable-asset", "audio/webm", bytes.Length, digest,
            "https://untrusted.example/should-not-be-followed");
        var handler = new RecordingHandler((request, _) =>
        {
            Assert.EndsWith("/v1/assets/portable-asset", request.RequestUri!.AbsoluteUri);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ByteArrayContent(bytes)
            });
        });
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));
        await using var destination = new MemoryStream();

        await client.DownloadVerifiedAsync(reference, destination);

        Assert.Equal(bytes, destination.ToArray());
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task DownloadVerifiedAsync_RejectsPortableReferenceWithoutIntegrityMetadata()
    {
        var reference = new ContentReference("portable-asset", "audio/webm", 10);
        using var http = new HttpClient(new RecordingHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));

        await Assert.ThrowsAsync<ArgumentException>(() =>
            client.DownloadVerifiedAsync(reference, new MemoryStream()));
    }

    [Fact]
    public async Task GetPublicationAsync_ReturnsOnlyMatchingValidatedManifest()
    {
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "channel-verified",
            new CreatorIdentity("creator-001"),
            "Verified channel",
            null,
            "unlisted",
            1,
            DateTimeOffset.UtcNow,
            new[] { new ContentReference("asset-001", "audio/webm", 4,
                "0123456789abcdef0123456789abcdef0123456789abcdef0123456789abcdef") });
        var handler = new RecordingHandler((request, _) =>
        {
            Assert.Equal(HttpMethod.Get, request.Method);
            Assert.EndsWith("/v1/channels/channel-verified", request.RequestUri!.AbsoluteUri);
            return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = JsonContent.Create(publication)
            });
        });
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));

        ChannelPublication result = await client.GetPublicationAsync("channel-verified");

        Assert.Equal(publication.PublicationId, result.PublicationId);
        Assert.Equal(publication.Content[0].Sha256, result.Content[0].Sha256);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task GetPublicationAsync_RejectsManifestWithDifferentRouteIdentity()
    {
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "different-channel",
            new CreatorIdentity("creator-001"),
            "Channel",
            null,
            "unlisted",
            1,
            DateTimeOffset.UtcNow);
        var handler = new RecordingHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(publication)
        }));
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://127.0.0.1:5080/"));

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            client.GetPublicationAsync("requested-channel"));
    }

    [Fact]
    public async Task DownloadVerifiedAsync_RejectsDigestMismatch()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("actual content");
        var expected = new MediaAssetDescriptor(
            "asset-mismatch", "audio/webm", bytes.Length,
            "0000000000000000000000000000000000000000000000000000000000000000");
        var handler = new RecordingHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
        }));
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://localhost:5080/"));

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            client.DownloadVerifiedAsync(expected, new MemoryStream()));
    }

    [Fact]
    public async Task DownloadVerifiedAsync_RejectsLengthMismatch()
    {
        byte[] bytes = Encoding.UTF8.GetBytes("short");
        string digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
        var expected = new MediaAssetDescriptor("asset-short", "audio/webm", bytes.Length + 1, digest);
        var handler = new RecordingHandler((_, _) => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new ByteArrayContent(bytes)
        }));
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://localhost:5080/"));

        await Assert.ThrowsAsync<InvalidDataException>(() =>
            client.DownloadVerifiedAsync(expected, new MemoryStream()));
    }

    [Fact]
    public async Task PublishAsync_UsesExplicitCallAndSendsValidatedManifest()
    {
        var handler = new RecordingHandler(async (request, cancellationToken) =>
        {
            Assert.Equal(HttpMethod.Put, request.Method);
            Assert.EndsWith("/v1/channels/channel-001", request.RequestUri!.AbsoluteUri);
            ChannelPublication? body = await request.Content!.ReadFromJsonAsync<ChannelPublication>(
                cancellationToken: cancellationToken);
            Assert.NotNull(body);
            Assert.Equal("channel-001", body!.PublicationId);
            return new HttpResponseMessage(HttpStatusCode.NoContent);
        });
        using var http = new HttpClient(handler);
        var client = new CreatorNodeClient(http, new Uri("http://localhost:5080"));
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "channel-001",
            new CreatorIdentity("creator-001"),
            "My recording",
            null,
            "unlisted",
            1,
            DateTimeOffset.UtcNow,
            new[] { new ContentReference("asset-001", "audio/webm", 10) });

        await client.PublishAsync(publication);

        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public void Constructor_RejectsRemotePlainHttpBecauseDevelopmentNodeIsUnauthenticated()
    {
        using var http = new HttpClient(new RecordingHandler((_, _) =>
            Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK))));

        Assert.Throws<ArgumentException>(() =>
            new CreatorNodeClient(http, new Uri("http://192.0.2.1:5080")));
    }

    private sealed class RecordingHandler : HttpMessageHandler
    {
        private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _send;

        public RecordingHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> send) =>
            _send = send;

        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            return _send(request, cancellationToken);
        }
    }
}
