namespace giraf_core_v2.Legacy.Abstractions;

// Turns whatever the app sends to identify itself into a v2 user ID.
// One implementation per app
public interface ILegacyCallerResolver
{
    Task<int?> ResolveCallerIdAsync(HttpContext http, CancellationToken ct);
}