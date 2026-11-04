using System.Net.Http.Json;

namespace giraf_core_v2.Tests.Integration;

public class CitizenEndpointTest(GirafApiFactory factory) : IClassFixture<GirafApiFactory>
{
    [Fact]
    public async Task GetCitizenById_ReturnsCorrectResult()
    {
        var seeded = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, seed: 42);
            var girafClass = await ClassSeeder.SeedAsync(db, seed: 42, organization);
            return await CitizenSeeder.SeedAsync(db, seed: 42, girafClass);
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seeded.citizen.Id}");
        
        response.EnsureSuccessStatusCode();
        var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
        Assert.NotNull(citizen);
        Assert.Equal(seeded.citizen.Id, citizen.UserId);
    }
}