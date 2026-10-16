using System.Reflection;

namespace giraf_core_v2.Legacy.Abstractions;

// Registers the legacy adapters and caller resolvers with dependency injection
public static class LegacyServiceCollectionExtensions
{
    // Registers every non-abstract ILegacyAdapter<,> and ILegacyCallerResolver implementation
    // in the core assembly, scoped, by concrete type
    public static IServiceCollection AddLegacyAdapters(this IServiceCollection services)
    => throw new NotImplementedException();

    // Same, for a given assembly. Lets tests register their fake adapters
    public static IServiceCollection AddLegacyAdapters(this IServiceCollection services, Assembly assembly)
    => throw new NotImplementedException();
}