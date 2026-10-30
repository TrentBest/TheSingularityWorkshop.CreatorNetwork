using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using CreatorNetwork.Contracts;
using CreatorNetwork.Node;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5080");

var dataDirectory = builder.Configuration["CREATORNETWORK_DATA"];
if (string.IsNullOrWhiteSpace(dataDirectory))
    dataDirectory = Path.Combine(Environment.CurrentDirectory, ".creatornetwork-data");

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

builder.Services.TryAddSingleton<ICreatorNetworkAccessPolicy, LoopbackDevelopmentAccessPolicy>();
builder.Services.AddSingleton(new FilePublicationStore(dataDirectory, jsonOptions));
builder.Services.AddSingleton(new FileMediaStore(Path.Combine(dataDirectory, "media")));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "local-demo", storage = "file-backed" }));

app.MapGet("/v1/channels/{publicationId}", async Task<IResult> (
    string publicationId,
    HttpContext context,
    ICreatorNetworkAccessPolicy accessPolicy,
    FilePublicationStore store,
    CancellationToken cancellationToken) =>
{
    if (!await accessPolicy.CanReadPublicationAsync(context, publicationId, cancellationToken))
        return Results.NotFound(new { error = "publication.not_found", publicationId });

    StoredPublication? document = await store.GetDocumentAsync(publicationId, cancellationToken);
    if (document is null)
        return Results.NotFound(new { error = "publication.not_found", publicationId });

    context.Response.Headers["ETag"] = document.ETag;
    if (context.Request.Headers["If-None-Match"].Any(value => string.Equals(value, document.ETag, StringComparison.Ordinal)))
        return Results.StatusCode(StatusCodes.Status304NotModified);

    return Results.Bytes(document.JsonBytes, "application/json; charset=utf-8");
});

app.MapPut("/v1/channels/{publicationId}", async Task<IResult> (
    string publicationId,
    ChannelPublication publication,
    HttpContext context,
    ICreatorNetworkAccessPolicy accessPolicy,
    FilePublicationStore store,
    CancellationToken cancellationToken) =>
{
    if (!string.Equals(publicationId, publication.PublicationId, StringComparison.Ordinal))
        return Results.BadRequest(new { error = "publication.id_mismatch", message = "The route identifier must match publicationId in the manifest." });

    IReadOnlyList<PublicationValidationError> errors = PublicationValidator.Validate(publication);
    if (errors.Count != 0)
        return Results.BadRequest(new { error = "publication.invalid", details = errors });

    if (!await accessPolicy.CanWritePublicationAsync(context, publication, cancellationToken))
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    try
    {
        await store.PutAsync(publication, cancellationToken);
        return Results.NoContent();
    }
    catch (PublicationRevisionConflictException exception)
    {
        return Results.Conflict(new
        {
            error = "publication.revision_conflict",
            publicationId = exception.PublicationId,
            currentRevision = exception.CurrentRevision,
            attemptedRevision = exception.AttemptedRevision
        });
    }
});

app.MapPut("/v1/assets/{assetId}", async Task<IResult> (
    string assetId,
    HttpContext context,
    ICreatorNetworkAccessPolicy accessPolicy,
    FileMediaStore store,
    CancellationToken cancellationToken) =>
{
    if (!await accessPolicy.CanWriteAssetAsync(context, assetId, cancellationToken))
        return Results.StatusCode(StatusCodes.Status403Forbidden);

    string mediaType = context.Request.ContentType ?? string.Empty;
    long? expectedLength = context.Request.ContentLength;
    string? expectedSha256 = context.Request.Headers["X-Content-SHA256"].FirstOrDefault();

    try
    {
        MediaAssetDescriptor descriptor = await store.PutAsync(
            assetId, mediaType, context.Request.Body, expectedLength, expectedSha256, cancellationToken);
        return Results.Created($"/v1/assets/{Uri.EscapeDataString(assetId)}", descriptor);
    }
    catch (MediaAssetAlreadyExistsException)
    {
        return Results.Conflict(new { error = "asset.already_exists", assetId });
    }
    catch (MediaAssetTooLargeException)
    {
        return Results.StatusCode(StatusCodes.Status413PayloadTooLarge);
    }
    catch (MediaAssetIntegrityException exception)
    {
        return Results.BadRequest(new { error = "asset.integrity_failed", message = exception.Message });
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = "asset.invalid", message = exception.Message });
    }
});

app.MapGet("/v1/assets/{assetId}", async Task<IResult> (
    string assetId,
    HttpContext context,
    ICreatorNetworkAccessPolicy accessPolicy,
    FileMediaStore store,
    CancellationToken cancellationToken) =>
{
    if (!await accessPolicy.CanReadAssetAsync(context, assetId, cancellationToken))
        return Results.NotFound(new { error = "asset.not_found", assetId });

    try
    {
        MediaAssetDescriptor? descriptor = await store.GetDescriptorAsync(assetId, cancellationToken);
        if (descriptor is null)
            return Results.NotFound(new { error = "asset.not_found", assetId });

        string etag = "\"sha256-" + descriptor.Sha256 + "\"";
        context.Response.Headers[HeaderNames.ETag] = etag;
        if (context.Request.Headers[HeaderNames.IfNoneMatch].Any(value =>
                string.Equals(value, etag, StringComparison.Ordinal)))
            return Results.StatusCode(StatusCodes.Status304NotModified);

        FileStream? stream = store.OpenRead(assetId);
        if (stream is null)
            return Results.NotFound(new { error = "asset.not_found", assetId });

        return Results.Stream(stream, contentType: descriptor.MediaType, enableRangeProcessing: true);
    }
    catch (ArgumentException exception)
    {
        return Results.BadRequest(new { error = "asset.invalid", message = exception.Message });
    }
});

app.Run();


public partial class Program { }
