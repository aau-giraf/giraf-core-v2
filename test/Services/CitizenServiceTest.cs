using giraf_core_v2.Tests.Seeders;
using Xunit.Abstractions;

namespace giraf_core_v2.Tests.Services;

public class CitizenServiceTest : TestBase
{
    public CitizenServiceTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task GetCitizenAsync_GetsCitizen()
    {
        // Create virtual DB
        await using var db = CreateDbContext();
        
        // Seed multiple citizen
        var seededCitizens = await CitizenSeeder.SeedManyAsync(db, Seed, 2);
        var citizen0 = seededCitizens[0].Citizen;
        var citizen1 = seededCitizens[1].Citizen;
        
        // Fetch citizen0 through the service
        db.ChangeTracker.Clear();
        var service = new CitizenService(db);
        var fetchedCitizen = await service.GetCitizenAsync(citizen0.UserId);
        
        // Assertions
        Assert.NotNull(fetchedCitizen);
        Assert.Equal(citizen0.UserId, fetchedCitizen.UserId);
        Assert.NotEqual(citizen1.UserId, fetchedCitizen.UserId);
    }

    [Fact]
    public async Task GetCitizens_GetsCitizens()
    {
        // Create virtual db
        await using var db = CreateDbContext();
        
        // Create two organizations with citizens
        var orgCitizens = await CitizenSeeder.SeedManyAsync(db, Seed, 3);
        var otherCitizens = await CitizenSeeder.SeedManyAsync(db, Seed + 100, 2);
        var orgId = orgCitizens[0].Citizen.Class.OrganizationId;
        db.ChangeTracker.Clear();
        
        // Service call to get citizens related to the organization id
        var service = new CitizenService(db);
        var fetched = await service.GetCitizensAsync(orgId);
        Assert.NotNull(fetched);
        
        Assert.Equal(
            orgCitizens.Select(c => c.Citizen.UserId).Order(),
            fetched.Select(c => c.UserId).Order());
        Assert.DoesNotContain(fetched, c => otherCitizens.Any(o => o.Citizen.UserId == c.UserId));
    }

    [Fact]
    public async Task DeleteCitizenAsync_DeletesCitizen()
    {
        
    }
}