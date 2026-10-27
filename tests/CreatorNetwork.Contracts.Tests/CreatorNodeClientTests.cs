using System.Net;
using System.Net.Http.Json;
using System.Text;
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
