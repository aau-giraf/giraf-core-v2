using System.Net;
using System.Net.Http.Json;

using Xunit.Abstractions;

namespace giraf_core_v2.Tests.Integration;

public class CitizenEndpointTest(GirafApiFactory factory, ITestOutputHelper output) : TestBase(output), IClassFixture<GirafApiFactory>
{
    [Fact]
    public async Task GetCitizenById_ReturnsCorrectResult()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed + 1, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.Id}");

        response.EnsureSuccessStatusCode();
        var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
        Assert.NotNull(citizen);
        Assert.Equal(seededCitizen.Id, citizen.UserId);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task DeleteCitizen_ReturnsCorrectResult()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });
        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"/citizens/{seededCitizen.Id}");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}