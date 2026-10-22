using System.Text.Json;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Abstractions;

// Route helpers that connect legacy URLs to adapters
public static class LegacyRouteExtensions
{
    // The old backends used the AddControllers() defaults: camelCase and enums as numbers.
    // Core's own JSON options write enums as strings, so legacy responses use these instead
    private static readonly JsonSerializerOptions LegacyJson = new(JsonSerializerDefaults.Web);

    // Creates an app's route group: the prefix, antiforgery disabled, and the app's caller resolver attached
    public static RouteGroupBuilder MapLegacyApp<TCallerResolver>(this WebApplication app, string prefix)
        where TCallerResolver : class, ILegacyCallerResolver
    {
        return app.MapGroup(prefix)
            .DisableAntiforgery()
            .WithMetadata(new LegacyAppMetadata(typeof(TCallerResolver), prefix));
    }

    // Maps one legacy GET endpoint to one adapter
    public static RouteHandlerBuilder MapLegacyGet<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => group.MapGet(pattern, HandleAsync<TRequest, TResponse, TAdapter>);

    // Maps one legacy POST endpoint to one adapter
    public static RouteHandlerBuilder MapLegacyPost<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => group.MapPost(pattern, HandleAsync<TRequest, TResponse, TAdapter>);

    // Maps one legacy PUT endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyPut<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => group.MapPut(pattern, HandleAsync<TRequest, TResponse, TAdapter>);

    // Maps one legacy PATCH endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyPatch<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => group.MapPatch(pattern, HandleAsync<TRequest, TResponse, TAdapter>);

    // Maps one legacy DELETE endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyDelete<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => group.MapDelete(pattern, HandleAsync<TRequest, TResponse, TAdapter>);

    // Runs for every legacy request: binds the request record, resolves the caller, calls the adapter and writes its result
    private static async Task<IResult> HandleAsync<TRequest, TResponse, TAdapter>(
        [AsParameters] TRequest request, [FromServices] TAdapter adapter, HttpContext http, CancellationToken ct)
        where TRequest : notnull
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
    {
        var legacyApp = http.GetEndpoint()?.Metadata.GetMetadata<LegacyAppMetadata>()
            ?? throw new InvalidOperationException("Legacy endpoints must be mapped on a group from MapLegacyApp");

        var resolver = (ILegacyCallerResolver)http.RequestServices.GetRequiredService(legacyApp.CallerResolverType);
        int? callerId = await resolver.ResolveCallerIdAsync(http, ct);
        string publicBaseUrl = $"{http.Request.Scheme}://{http.Request.Host}{http.Request.PathBase}{legacyApp.Prefix}";

        var result = await adapter.HandleAsync(request, new LegacyCallContext(callerId, publicBaseUrl), ct);

        if (result.Location is not null)
        {
            http.Response.Headers.Location = result.Location;
        }

        return ToHttpResult(result);
    }

    // Writes the result the way the old MVC controllers did
    private static IResult ToHttpResult<T>(LegacyResult<T> result)
    {
        object? body = result.StatusCode < 400 ? result.Value : result.ErrorBody;

        return body switch
        {
            null or NoBody => Results.StatusCode(result.StatusCode),
            LegacyFile file => Results.File(file.Content, file.ContentType, file.FileName),
            // MVC writes strings as plain text, not as JSON strings
            string text => Results.Text(text, "text/plain; charset=utf-8", statusCode: result.StatusCode),
            _ => Results.Json(body, LegacyJson, statusCode: result.StatusCode),
        };
    }
}

// Stored on the app's route group by MapLegacyApp; read by every MapLegacy* endpoint at request time
internal sealed record LegacyAppMetadata(Type CallerResolverType, string Prefix);