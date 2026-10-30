using System.Net;
using System.Net.Http.Json;
using CreatorNetwork.Contracts;
using CreatorNetwork.Node;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class CreatorNodeAuthorizationTests
{
    [Fact]
    public async Task Node_EnforcesHostPolicyForPublicationAndAssetReadsAndWrites()
    {
        string directory = Path.Combine(Path.GetTempPath(), "creator-network-node-tests", Guid.NewGuid().ToString("N"));
        var policy = new DenyAllAccessPolicy();

        try
        {
            using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("CREATORNETWORK_DATA", directory);
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICreatorNetworkAccessPolicy>();
                    services.AddSingleton<ICreatorNetworkAccessPolicy>(policy);
                });
            });
            using HttpClient client = factory.CreateClient();

            var publication = new ChannelPublication(
                ChannelPublication.CurrentSchema,
                "publication:denied",
                new CreatorIdentity("creator:denied"),
                "Denied publication",
                null,
                "private",
                1,
                DateTimeOffset.UtcNow);

            using HttpResponseMessage publicationWrite = await client.PutAsJsonAsync(
                "/v1/channels/publication%3Adenied", publication);
            using HttpResponseMessage publicationRead = await client.GetAsync(
                "/v1/channels/publication%3Adenied");

            using var assetBody = new ByteArrayContent([1, 2, 3]);
            assetBody.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
            using HttpResponseMessage assetWrite = await client.PutAsync("/v1/assets/asset-denied", assetBody);
            using HttpResponseMessage assetRead = await client.GetAsync("/v1/assets/asset-denied");

            Assert.Equal(HttpStatusCode.Forbidden, publicationWrite.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, publicationRead.StatusCode);
            Assert.Equal(HttpStatusCode.Forbidden, assetWrite.StatusCode);
            Assert.Equal(HttpStatusCode.NotFound, assetRead.StatusCode);
            Assert.Equal(4, policy.DecisionCount);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    private sealed class DenyAllAccessPolicy : ICreatorNetworkAccessPolicy
    {
        private int _decisionCount;
        public int DecisionCount => Volatile.Read(ref _decisionCount);

        public ValueTask<bool> CanReadPublicationAsync(
            HttpContext context, string publicationId, CancellationToken cancellationToken) => Deny();

        public ValueTask<bool> CanWritePublicationAsync(
            HttpContext context, ChannelPublication publication, CancellationToken cancellationToken) => Deny();

        public ValueTask<bool> CanReadAssetAsync(
            HttpContext context, string assetId, CancellationToken cancellationToken) => Deny();

        public ValueTask<bool> CanWriteAssetAsync(
            HttpContext context, string assetId, CancellationToken cancellationToken) => Deny();

        private ValueTask<bool> Deny()
        {
            Interlocked.Increment(ref _decisionCount);
            return ValueTask.FromResult(false);
        }
    }
}
