using System.Reflection;

namespace giraf_core_v2.Legacy.Abstractions;

// Registers the legacy adapters and caller resolvers with dependency injection
public static class LegacyServiceCollectionExtensions
{
    // Registers every non-abstract ILegacyAdapter<,> and ILegacyCallerResolver implementation
    // in the core assembly, scoped, by concrete type
    public static IServiceCollection AddLegacyAdapters(this IServiceCollection services)
    => services.AddLegacyAdapters(typeof(LegacyServiceCollectionExtensions).Assembly);

    // Same, for a given assembly. Lets tests register their fake adapters
    public static IServiceCollection AddLegacyAdapters(this IServiceCollection services, Assembly assembly)
    {
        var legacyTypes = assembly.GetTypes()
            .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericTypeDefinition: false })
            .Where(type => typeof(ILegacyCallerResolver).IsAssignableFrom(type) || IsAdapter(type));

        foreach (var type in legacyTypes)
        {
            services.AddScoped(type);
        }

        return services;
    }

    private static bool IsAdapter(Type type)
    {
        return type.GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ILegacyAdapter<,>));
    }
}