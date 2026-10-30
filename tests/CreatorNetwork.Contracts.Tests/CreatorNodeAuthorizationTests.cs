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


    [Fact]
    public async Task PublicationWrites_RequireAndEnforceConditionalEtags()
    {
        string directory = Path.Combine(Path.GetTempPath(), "creator-network-node-tests", Guid.NewGuid().ToString("N"));

        try
        {
            using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.UseSetting("CREATORNETWORK_DATA", directory);
                builder.ConfigureTestServices(services =>
                {
                    services.RemoveAll<ICreatorNetworkAccessPolicy>();
                    services.AddSingleton<ICreatorNetworkAccessPolicy, AllowAllAccessPolicy>();
                });
            });
            using HttpClient client = factory.CreateClient();
            const string route = "/v1/channels/publication%3Aconditional";

            using var missingPrecondition = await client.PutAsJsonAsync(route, CreatePublication(1));
            Assert.Equal((HttpStatusCode)428, missingPrecondition.StatusCode);

            using var create = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = JsonContent.Create(CreatePublication(1))
            };
            create.Headers.TryAddWithoutValidation("If-None-Match", "*");
            using HttpResponseMessage created = await client.SendAsync(create);
            Assert.Equal(HttpStatusCode.NoContent, created.StatusCode);
            string originalETag = Assert.Single(created.Headers.ETag is null
                ? Array.Empty<string>()
                : new[] { created.Headers.ETag.ToString() });

            using var get = await client.GetAsync(route);
            Assert.Equal(HttpStatusCode.OK, get.StatusCode);
            string fetchedETag = Assert.Single(get.Headers.ETag is null
                ? Array.Empty<string>()
                : new[] { get.Headers.ETag.ToString() });
            Assert.Equal(originalETag, fetchedETag);

            using var duplicatePrecondition = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = JsonContent.Create(CreatePublication(2))
            };
            duplicatePrecondition.Headers.TryAddWithoutValidation("If-Match", new[] { originalETag, originalETag });
            using HttpResponseMessage duplicateResult = await client.SendAsync(duplicatePrecondition);
            Assert.Equal(HttpStatusCode.BadRequest, duplicateResult.StatusCode);

            using var update = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = JsonContent.Create(CreatePublication(2))
            };
            update.Headers.TryAddWithoutValidation("If-Match", originalETag);
            using HttpResponseMessage updated = await client.SendAsync(update);
            Assert.Equal(HttpStatusCode.NoContent, updated.StatusCode);
            string newETag = updated.Headers.ETag!.ToString();
            Assert.NotEqual(originalETag, newETag);

            using var stale = new HttpRequestMessage(HttpMethod.Put, route)
            {
                Content = JsonContent.Create(CreatePublication(3))
            };
            stale.Headers.TryAddWithoutValidation("If-Match", originalETag);
            using HttpResponseMessage staleResult = await client.SendAsync(stale);
            Assert.Equal(HttpStatusCode.PreconditionFailed, staleResult.StatusCode);

            using var staleRead = await client.GetAsync(route);
            ChannelPublication? current = await staleRead.Content.ReadFromJsonAsync<ChannelPublication>();
            Assert.NotNull(current);
            Assert.Equal(2, current!.Revision);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    private static ChannelPublication CreatePublication(long revision) =>
        new(
            ChannelPublication.CurrentSchema,
            "publication:conditional",
            new CreatorIdentity("creator:conditional"),
            "Conditional publication",
            null,
            "private",
            revision,
            DateTimeOffset.UtcNow);

    private sealed class AllowAllAccessPolicy : ICreatorNetworkAccessPolicy
    {
        public ValueTask<bool> CanReadPublicationAsync(HttpContext context, string publicationId, CancellationToken cancellationToken) =>
            ValueTask.FromResult(true);
        public ValueTask<bool> CanWritePublicationAsync(HttpContext context, ChannelPublication publication, CancellationToken cancellationToken) =>
            ValueTask.FromResult(true);
        public ValueTask<bool> CanReadAssetAsync(HttpContext context, string assetId, CancellationToken cancellationToken) =>
            ValueTask.FromResult(true);
        public ValueTask<bool> CanWriteAssetAsync(HttpContext context, string assetId, CancellationToken cancellationToken) =>
            ValueTask.FromResult(true);
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
