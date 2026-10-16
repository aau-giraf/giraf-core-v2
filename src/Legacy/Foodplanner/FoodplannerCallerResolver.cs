using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Foodplanner;

// Works out which v2 user a Foodplanner request comes from. Depends on the open login/token decision
public sealed class FoodplannerCallerResolver : ILegacyCallerResolver
{
    public Task<int?> ResolveCallerIdAsync(HttpContext http, CancellationToken ct)
    => throw new NotImplementedException();
}