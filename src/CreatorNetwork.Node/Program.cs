using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using CreatorNetwork.Contracts;
using CreatorNetwork.Node;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://127.0.0.1:5080");

var dataDirectory = Environment.GetEnvironmentVariable("CREATORNETWORK_DATA");
if (string.IsNullOrWhiteSpace(dataDirectory))
    dataDirectory = Path.Combine(AppContext.BaseDirectory, "data");

var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web)
{
    WriteIndented = false,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
};

builder.Services.AddSingleton(new FilePublicationStore(dataDirectory, jsonOptions));

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "local-demo", storage = "file-backed" }));

app.MapGet("/v1/channels/{publicationId}", async (
    string publicationId,
    HttpContext context,
    FilePublicationStore store,
    CancellationToken cancellationToken) =>
{
    ChannelPublication? publication = await store.GetAsync(publicationId, cancellationToken);
    if (publication is null)
        return Results.NotFound(new { error = "publication.not_found", publicationId });

    byte[] bytes = JsonSerializer.SerializeToUtf8Bytes(publication, jsonOptions);
    string etag = "\"" + Convert.ToHexString(SHA256.HashData(bytes)).ToLowerInvariant() + "\"";
    context.Response.Headers.ETag = etag;
    if (context.Request.Headers.IfNoneMatch.Any(value => string.Equals(value, etag, StringComparison.Ordinal)))
        return Results.StatusCode(StatusCodes.Status304NotModified);

    return Results.Bytes(bytes, "application/json; charset=utf-8");
});

app.MapPut("/v1/channels/{publicationId}", async (
    string publicationId,
    ChannelPublication publication,
    FilePublicationStore store,
    CancellationToken cancellationToken) =>
{
    if (!string.Equals(publicationId, publication.PublicationId, StringComparison.Ordinal))
        return Results.BadRequest(new { error = "publication.id_mismatch", message = "The route identifier must match publicationId in the manifest." });

    IReadOnlyList<PublicationValidationError> errors = PublicationValidator.Validate(publication);
    if (errors.Count != 0)
        return Results.BadRequest(new { error = "publication.invalid", details = errors });

    await store.PutAsync(publication, cancellationToken);
    return Results.NoContent();
});

app.Run();
