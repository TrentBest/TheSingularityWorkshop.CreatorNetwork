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
    [Fact]
    public async Task PublicationUpdates_RequireStrictlyIncreasingRevisions()
    {
        string directory = Path.Combine(Path.GetTempPath(), "creator-network-tests", Guid.NewGuid().ToString("N"));
        var store = new FilePublicationStore(directory, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        try
        {
            await store.PutAsync(CreatePublication(4), CancellationToken.None);

            var duplicate = await Assert.ThrowsAsync<PublicationRevisionConflictException>(() =>
                store.PutAsync(CreatePublication(4), CancellationToken.None));
            Assert.Equal(4, duplicate.CurrentRevision);
            Assert.Equal(4, duplicate.AttemptedRevision);

            var stale = await Assert.ThrowsAsync<PublicationRevisionConflictException>(() =>
                store.PutAsync(CreatePublication(3), CancellationToken.None));
            Assert.Equal(4, stale.CurrentRevision);
            Assert.Equal(3, stale.AttemptedRevision);

            await store.PutAsync(CreatePublication(5), CancellationToken.None);
            ChannelPublication? current = await store.GetAsync("publication:revision", CancellationToken.None);
            Assert.NotNull(current);
            Assert.Equal(5, current!.Revision);
        }
        finally
        {
            if (Directory.Exists(directory))
                Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task PublicationUpdates_ConcurrentStaleWritersCannotReplaceNewerRevision()
    {
        string directory = Path.Combine(Path.GetTempPath(), "creator-network-tests", Guid.NewGuid().ToString("N"));
        var store = new FilePublicationStore(directory, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        try
        {
            await store.PutAsync(CreatePublication(1), CancellationToken.None);

            Task lower = store.PutAsync(CreatePublication(2), CancellationToken.None);
            Task higher = store.PutAsync(CreatePublication(3), CancellationToken.None);
            await Task.WhenAll(
                lower.ContinueWith(task => task.IsFaulted && task.Exception!.GetBaseException() is PublicationRevisionConflictException,
                    TaskScheduler.Default),
                higher.ContinueWith(task => task.IsFaulted && task.Exception!.GetBaseException() is PublicationRevisionConflictException,
                    TaskScheduler.Default));

            ChannelPublication? current = await store.GetAsync("publication:revision", CancellationToken.None);
            Assert.NotNull(current);
            Assert.Equal(3, current!.Revision);
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
            "publication:revision",
            new CreatorIdentity("creator:revision"),
            "Revision test",
            null,
            "private",
            revision,
            DateTimeOffset.UtcNow);

}
