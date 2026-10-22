namespace giraf_core_v2.Legacy.Abstractions;

// Route helpers that connect legacy URLs to adapters
public static class LegacyRouteExtensions
{
    // Creates an app's route group: the prefix, antiforgery disabled, and the app's caller resolver attached
    public static RouteGroupBuilder MapLegacyApp<TCallerResolver>(this WebApplication app, string prefix)
        where TCallerResolver : class, ILegacyCallerResolver
        => throw new NotImplementedException();

    // Maps one legacy GET endpoint to one adapter
    public static RouteHandlerBuilder MapLegacyGet<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => throw new NotImplementedException();

    // Maps one legacy POST endpoint to one adapter
    public static RouteHandlerBuilder MapLegacyPost<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => throw new NotImplementedException();

    // Maps one legacy PUT endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyPut<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => throw new NotImplementedException();

    // Maps one legacy PATCH endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyPatch<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => throw new NotImplementedException();

    // Maps one legacy DELETE endpoint to an adapter
    public static RouteHandlerBuilder MapLegacyDelete<TRequest, TResponse, TAdapter>(
        this RouteGroupBuilder group, string pattern)
        where TAdapter : class, ILegacyAdapter<TRequest, TResponse>
        => throw new NotImplementedException();
}

// Stored on the app's route group by MapLegacyApp; read by every MapLegacy* endpoint at request time
internal sealed record LegacyAppMetadata(Type CallerResolverType, string Prefix);