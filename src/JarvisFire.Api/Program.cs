using Npgsql;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("Postgres")
    ?? Environment.GetEnvironmentVariable("JARVISFIRE_POSTGRES");
if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Configure ConnectionStrings:Postgres or JARVISFIRE_POSTGRES.");
builder.Services.AddSingleton(NpgsqlDataSource.Create(connectionString));
builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
var app = builder.Build();
app.MapGet("/api/v1/health", () => Results.Ok(new { status = "ok", service = "jarvisfire-api", version = "0.1-prototype" }));
// Prototype endpoint: disabled unless a server-side key is configured. Replace with role-based JWT auth before deployment.
var apiKey = Environment.GetEnvironmentVariable("JARVISFIRE_DEV_API_KEY");
if (string.IsNullOrWhiteSpace(apiKey) || apiKey.Length < 32)
    throw new InvalidOperationException("Set JARVISFIRE_DEV_API_KEY (minimum 32 characters). This is a development-only API.");
var protectedApi = app.MapGroup("/api/v1").AddEndpointFilter(async (context, next) =>
{
    var provided = context.HttpContext.Request.Headers["X-Dev-Api-Key"].ToString();
    if (provided.Length != apiKey.Length ||
        !System.Security.Cryptography.CryptographicOperations.FixedTimeEquals(
            System.Text.Encoding.UTF8.GetBytes(provided), System.Text.Encoding.UTF8.GetBytes(apiKey)))
        return Results.Unauthorized();
    return await next(context);
});
protectedApi.MapGet("/incidents", async (NpgsqlDataSource db, CancellationToken ct) =>
{
    var results = new List<IncidentDto>();
    await using var cmd = db.CreateCommand("SELECT id, created_at, address, description, status FROM incidents ORDER BY created_at DESC LIMIT 100");
    await using var reader = await cmd.ExecuteReaderAsync(ct);
    while (await reader.ReadAsync(ct))
        results.Add(new(reader.GetInt64(0), reader.GetFieldValue<DateTime>(1), reader.GetString(2), reader.GetString(3), reader.GetString(4)));
    return Results.Ok(results);
});
protectedApi.MapPost("/incidents", async (CreateIncident input, NpgsqlDataSource db, CancellationToken ct) =>
{
    if (string.IsNullOrWhiteSpace(input.Address) || input.Address.Length > 300 || (input.Description?.Length ?? 0) > 2000)
        return Results.BadRequest(new { error = "Invalid address or description" });
    await using var cmd = db.CreateCommand("INSERT INTO incidents (address, description) VALUES ($1, $2) RETURNING id");
    cmd.Parameters.AddWithValue(input.Address.Trim());
    cmd.Parameters.AddWithValue(input.Description?.Trim() ?? "");
    var id = (long)(await cmd.ExecuteScalarAsync(ct) ?? 0L);
    return Results.Created($"/api/v1/incidents/{id}", new { id });
});
app.Run();
public sealed record CreateIncident(string Address, string? Description);
public sealed record IncidentDto(long Id, DateTime CreatedAt, string Address, string Description, string Status);
