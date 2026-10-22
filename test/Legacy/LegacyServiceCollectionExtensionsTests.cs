using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner;

using Microsoft.Extensions.DependencyInjection;

public class LegacyServiceCollectionExtensionsTests
{
    [Fact]
    public void AddLegacyAdapters_RegistersAdaptersAndResolversScopedByConcreteType()
    {
        var services = new ServiceCollection();

        services.AddLegacyAdapters(typeof(LegacyServiceCollectionExtensionsTests).Assembly);

        Assert.Contains(services, s => s.ServiceType == typeof(GetThingAdapter) && s.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services, s => s.ServiceType == typeof(FakeCallerResolver) && s.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddLegacyAdapters_SkipsAbstractTypes()
    {
        var services = new ServiceCollection();

        services.AddLegacyAdapters(typeof(LegacyServiceCollectionExtensionsTests).Assembly);

        Assert.DoesNotContain(services, s => s.ServiceType == typeof(AbstractThingAdapter));
    }

    [Fact]
    public void AddLegacyAdapters_WithoutAssembly_ScansCore()
    {
        var services = new ServiceCollection();

        services.AddLegacyAdapters();

        Assert.Contains(services, s => s.ServiceType == typeof(FoodplannerCallerResolver));
        Assert.DoesNotContain(services, s => s.ServiceType == typeof(GetThingAdapter));
    }
}