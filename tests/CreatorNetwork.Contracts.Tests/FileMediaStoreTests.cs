using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CreatorNetwork.Node;
using Xunit;

namespace CreatorNetwork.Contracts.Tests;

public sealed class FileMediaStoreTests
{
    [Fact]
    public async Task UploadStreamsBytesAndPersistsDescriptorAcrossStoreInstances()
    {
        string directory = NewDirectory();
        try
        {
            byte[] bytes = Encoding.UTF8.GetBytes("creator media payload");
            string digest = Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant();
            var first = new FileMediaStore(directory);

            var saved = await first.PutAsync(
                "asset:episode-1", "video/mp4", new MemoryStream(bytes),
                bytes.Length, digest, CancellationToken.None);

            Assert.Equal(bytes.Length, saved.LengthBytes);
            Assert.Equal(digest, saved.Sha256);

            var reopened = new FileMediaStore(directory);
            var loaded = await reopened.GetDescriptorAsync("asset:episode-1", CancellationToken.None);
            Assert.NotNull(loaded);
            Assert.Equal(saved.AssetId, loaded!.AssetId);
            Assert.Equal(saved.MediaType, loaded.MediaType);

            await using var stream = reopened.OpenRead("asset:episode-1");
            Assert.NotNull(stream);
            using var output = new MemoryStream();
            await stream!.CopyToAsync(output);
            Assert.Equal(bytes, output.ToArray());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task DigestMismatchDoesNotFinalizeAsset()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory);
            byte[] bytes = Encoding.UTF8.GetBytes("payload");

            await Assert.ThrowsAsync<MediaAssetIntegrityException>(() => store.PutAsync(
                "asset:bad-digest", "application/octet-stream", new MemoryStream(bytes),
                bytes.Length, new string('0', 64), CancellationToken.None));

            Assert.Null(await store.GetDescriptorAsync("asset:bad-digest", CancellationToken.None));
            Assert.Null(store.OpenRead("asset:bad-digest"));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task DuplicateAssetIdIsRejectedWithoutReplacingOriginal()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory);
            byte[] original = Encoding.UTF8.GetBytes("original");
            byte[] replacement = Encoding.UTF8.GetBytes("replacement");

            await store.PutAsync("asset:fixed", "text/plain", new MemoryStream(original),
                original.Length, null, CancellationToken.None);

            await Assert.ThrowsAsync<MediaAssetAlreadyExistsException>(() => store.PutAsync(
                "asset:fixed", "text/plain", new MemoryStream(replacement),
                replacement.Length, null, CancellationToken.None));

            await using var stream = store.OpenRead("asset:fixed");
            using var output = new MemoryStream();
            await stream!.CopyToAsync(output);
            Assert.Equal(original, output.ToArray());
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task UploadLargerThanConfiguredLimitIsRejectedAndNotVisible()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory, maximumAssetBytes: 4);

            await Assert.ThrowsAsync<MediaAssetTooLargeException>(() => store.PutAsync(
                "asset:large", "application/octet-stream", new MemoryStream(new byte[5]),
                expectedLength: null, expectedSha256: null, CancellationToken.None));

            Assert.Null(await store.GetDescriptorAsync("asset:large", CancellationToken.None));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task DeclaredLengthMismatchDoesNotFinalizeAsset()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory);
            byte[] bytes = Encoding.UTF8.GetBytes("length mismatch");

            await Assert.ThrowsAsync<MediaAssetIntegrityException>(() => store.PutAsync(
                "asset:wrong-length", "text/plain", new MemoryStream(bytes),
                bytes.Length + 1, null, CancellationToken.None));

            Assert.Null(await store.GetDescriptorAsync("asset:wrong-length", CancellationToken.None));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task ConcurrentUploadsForOneIdFinalizeAtMostOneAsset()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory);
            Task<MediaAssetDescriptor> first = store.PutAsync(
                "asset:concurrent", "text/plain", new MemoryStream(Encoding.UTF8.GetBytes("first")),
                null, null, CancellationToken.None);
            Task<MediaAssetDescriptor> second = store.PutAsync(
                "asset:concurrent", "text/plain", new MemoryStream(Encoding.UTF8.GetBytes("second")),
                null, null, CancellationToken.None);

            async Task<bool> IsAcceptedAsync(Task<MediaAssetDescriptor> upload)
            {
                try
                {
                    await upload;
                    return true;
                }
                catch (MediaAssetAlreadyExistsException)
                {
                    return false;
                }
            }

            bool[] results = await Task.WhenAll(IsAcceptedAsync(first), IsAcceptedAsync(second));
            Assert.Single(Array.FindAll(results, accepted => accepted));
            Assert.Single(Array.FindAll(results, accepted => !accepted));
            Assert.NotNull(await store.GetDescriptorAsync("asset:concurrent", CancellationToken.None));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    [Fact]
    public async Task PathLikeAssetIdCannotEscapeStoreDirectory()
    {
        string directory = NewDirectory();
        try
        {
            var store = new FileMediaStore(directory);
            byte[] bytes = Encoding.UTF8.GetBytes("safe");
            await store.PutAsync("../../outside", "text/plain", new MemoryStream(bytes),
                bytes.Length, null, CancellationToken.None);

            Assert.Equal(2, Directory.GetFiles(directory).Length);
            Assert.False(File.Exists(Path.Combine(Directory.GetParent(directory)!.FullName, "outside.asset")));
        }
        finally
        {
            Directory.Delete(directory, recursive: true);
        }
    }

    private static string NewDirectory()
    {
        string directory = Path.Combine(Path.GetTempPath(), "CreatorNetwork.MediaTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(directory);
        return directory;
    }
}
