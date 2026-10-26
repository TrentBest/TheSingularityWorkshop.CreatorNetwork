using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CreatorNetwork.Contracts;
using CreatorNetwork.Node;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class FilePublicationStoreTests
{
    [Fact]
    public async Task PublicationSurvivesStoreRecreation()
    {
        string directory = Path.Combine(Path.GetTempPath(), "creator-network-tests", Guid.NewGuid().ToString("N"));
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var publication = new ChannelPublication(
            ChannelPublication.CurrentSchema,
            "publication:persistence",
            new CreatorIdentity("creator:persistence", "Persistence test"),
            "Persistence",
            "Round-trip test",
            "public",
            1,
            DateTimeOffset.UtcNow,
            new[] { new ContentReference("content:video", "video/mp4", 2048) });

        try
        {
            var firstStore = new FilePublicationStore(directory, options);
            await firstStore.PutAsync(publication, CancellationToken.None);

            var secondStore = new FilePublicationStore(directory, options);
            ChannelPublication? loaded = await secondStore.GetAsync(publication.PublicationId, CancellationToken.None);

            Assert.NotNull(loaded);
            Assert.Equal(publication.PublicationId, loaded!.PublicationId);
            Assert.Equal(publication.Creator.Id, loaded.Creator.Id);
            Assert.Single(loaded.Content);
            Assert.Equal("content:video", loaded.Content[0].Id);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }
}
