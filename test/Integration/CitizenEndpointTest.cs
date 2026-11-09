using System.Net;
using System.Net.Http.Json;

namespace giraf_core_v2.Tests.Integration;

public class CitizenEndpointTest(GirafApiFactory factory, ITestOutputHelper output) : TestBase(output), IClassFixture<GirafApiFactory>
{
    [Fact]
    public async Task GetCitizenById_ReturnsCorrectResult()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.Id}");

        try
        {
            response.EnsureSuccessStatusCode();
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
            Assert.NotNull(citizen);
            Assert.Equal(seededCitizen.Id, citizen.UserId);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }

    [Fact]
    public async Task GetCitizenById_ReturnsNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.Id + 1}");

        try
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }

    [Fact]
    public async Task CreateCitizen_ReturnsCreatedCitizen()
    {
        var seededClass = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            return await ClassSeeder.SeedAsync(db, Seed, organization.Id);
        });

        CreateCitizenDTO requestBody = new Faker<CreateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, seededClass.Id)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .RuleFor(u => u.Password, f => f.Random.Word());


        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/citizens", requestBody);

        try
        {
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
            Assert.NotNull(citizen);
            Assert.Equal(requestBody.FirstName, citizen.FirstName);
            Assert.Equal(requestBody.LastName, citizen.LastName);
            Assert.Equal(requestBody.ClassId, citizen.ClassId);
            Assert.Equal(requestBody.Username, citizen.Username);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }

    [Fact]
    public async Task CreateCitizen_ReturnsBadRequest()
    {
        var seededClass = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            return await ClassSeeder.SeedAsync(db, Seed, organization.Id);
        });

        CreateCitizenDTO requestBody = new Faker<CreateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, seededClass.Id + 1)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .RuleFor(u => u.Password, f => f.Random.Word());


        var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/citizens", requestBody);

        try
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }


    [Theory]
    [InlineData(true, true, true, true)]
    [InlineData(true, false, true, true)]
    [InlineData(true, true, false, true)]
    [InlineData(true, true, true, false)]
    [InlineData(false, true, true, true)]
    [InlineData(true, true, false, false)]
    [InlineData(true, false, true, false)]
    [InlineData(true, false, false, true)]
    [InlineData(false, true, true, false)]
    [InlineData(false, true, false, true)]
    [InlineData(true, false, false, false)]
    [InlineData(false, true, false, false)]
    [InlineData(false, false, true, false)]
    [InlineData(false, false, false, true)]
    [InlineData(false, false, false, false)]
    public async Task UpdateCitizen_ReturnsCreatedCitizen(bool isFirstNameChanged, bool isLastNameChanged, bool isClassChanged, bool isUsernameChanged)
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });

        UpdateCitizenDTO requestBody = new Faker<UpdateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => isFirstNameChanged ? f.Person.FirstName : null)
            .RuleFor(u => u.LastName, f => isLastNameChanged ? f.Person.LastName : null)
            .RuleFor(u => u.ClassId, isClassChanged ? seededCitizen.Id : null)
            .RuleFor(u => u.Username, f => isUsernameChanged ? f.Person.UserName : null);

        var client = factory.CreateClient();

        var response = await client.PatchAsJsonAsync($"/citizens/{seededCitizen.Id}", requestBody);

        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
            Assert.NotNull(citizen);
            Assert.Equal(isFirstNameChanged ? requestBody.FirstName : seededCitizen.FirstName, citizen.FirstName);
            Assert.Equal(isLastNameChanged ? requestBody.LastName : seededCitizen.LastName, citizen.LastName);
            Assert.Equal(isClassChanged ? requestBody.ClassId : seededCitizen.ClassId, citizen.ClassId);
            Assert.Equal(isUsernameChanged ? requestBody.Username : seededCitizen.Username, citizen.Username);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }

    [Fact]
    public async Task UpdateCitizen_ReturnsBadRequest()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });

        var client = factory.CreateClient();

        var response = await client.PatchAsync($"/citizens/{seededCitizen.Id}", null);

        try
        {
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
    }


    [Fact]
    public async Task UpdateCitizen_ReturnsUserNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });

        var client = factory.CreateClient();


        UpdateCitizenDTO updateCitizenDTO = new Faker<UpdateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, seededCitizen.Id)
            .RuleFor(u => u.Username, f => f.Person.UserName);

        var response = await client.PatchAsJsonAsync($"/citizens/{seededCitizen.Id + 1}", updateCitizenDTO);

        try
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync(); //Resets the DB for each test
        }
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
        try
        {
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync();
        }
    }


    [Fact]
    public async Task DeleteCitizen_ReturnsNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await OrganizationSeeder.SeedAsync(db, Seed);
            var class_ = await ClassSeeder.SeedAsync(db, Seed, organization.Id);
            return await CitizenSeeder.SeedAsync(db, Seed, class_.Id);
        });
        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"/citizens/{seededCitizen.Id + 1}");

        try
        {
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
        finally
        {
            await factory.ResetDatabaseAsync();
        }
    }
}