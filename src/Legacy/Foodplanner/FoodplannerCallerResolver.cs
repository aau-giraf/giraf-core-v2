using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Foodplanner;

// Works out which v2 user a Foodplanner request comes from. Depends on the open login/token decision
public sealed class FoodplannerCallerResolver : ILegacyCallerResolver
{
    // TEMPORARY until v2 has login: the caller sends their v2 user ID in this header.
    // Anyone can claim any ID, so replace this before the Foodplanner app uses it for real
    private const string DevUserIdHeader = "X-Dev-User-Id";

    public Task<int?> ResolveCallerIdAsync(HttpContext http, CancellationToken ct)
    {
        bool valid = int.TryParse(http.Request.Headers[DevUserIdHeader], out int id) && id > 0;
        return Task.FromResult<int?>(valid ? id : null);
    }
}