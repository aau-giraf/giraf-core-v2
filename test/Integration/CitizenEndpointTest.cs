using System.Net;
using System.Net.Http.Json;

namespace giraf_core_v2.Tests.Integration;

public class CitizenEndpointTest(GirafApiFactory factory, ITestOutputHelper output) : TestBase(output), IClassFixture<GirafApiFactory>
{
    private readonly UserSeeder _userSeeder = new();
    private readonly ClassSeeder _classSeeder = new();
    private readonly OrganizationSeeder _orgSeeder = new();

    [Fact]
    public async Task GetCitizenById_ReturnsCorrectResult()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(ClassId: class_.Id, Type: UserType.Citizen));
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.User.Id}");

        try
        {
            response.EnsureSuccessStatusCode();
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
            Assert.NotNull(citizen);
            Assert.Equal(seededCitizen.User.Id, citizen.UserId);
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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: class_.Id));
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.User.Id + 1}");

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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            return await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            return await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: class_.Id));
        });
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        UpdateCitizenDTO requestBody = new Faker<UpdateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => isFirstNameChanged ? f.Person.FirstName : null)
            .RuleFor(u => u.LastName, f => isLastNameChanged ? f.Person.LastName : null)
            .RuleFor(u => u.ClassId, isClassChanged ? citizen.Id : null)
            .RuleFor(u => u.Username, f => isUsernameChanged ? f.Person.UserName : null);

        var client = factory.CreateClient();

        var response = await client.PatchAsJsonAsync($"/citizens/{citizen.Id}", requestBody);

        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var citizenJson = await response.Content.ReadFromJsonAsync<GetCitizenDTO>();
            Assert.NotNull(citizenJson);
            Assert.Equal(isFirstNameChanged ? requestBody.FirstName : citizen.FirstName, citizenJson.FirstName);
            Assert.Equal(isLastNameChanged ? requestBody.LastName : citizen.LastName, citizenJson.LastName);
            Assert.Equal(isClassChanged ? requestBody.ClassId : citizen.ClassId, citizenJson.ClassId);
            Assert.Equal(isUsernameChanged ? requestBody.Username : citizen.Username, citizenJson.Username);
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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: class_.Id));
        });

        var client = factory.CreateClient();

        var response = await client.PatchAsync($"/citizens/{seededCitizen.User.Id}", null);

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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(ClassId: class_.Id));
        });
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);
        var client = factory.CreateClient();


        UpdateCitizenDTO updateCitizenDTO = new Faker<UpdateCitizenDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, citizen.Id)
            .RuleFor(u => u.Username, f => f.Person.UserName);

        var response = await client.PatchAsJsonAsync($"/citizens/{citizen.Id + 1}", updateCitizenDTO);

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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: class_.Id));
        });
        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"/citizens/{seededCitizen.User.Id}");

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
            var organization = await _orgSeeder.SeedAsync(db, Seed);
            var class_ = await _classSeeder.SeedAsync(db, Seed, new(OrgId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: class_.Id));
        });
        var client = factory.CreateClient();
        var response = await client.DeleteAsync($"/citizens/{seededCitizen.User.Id + 1}");

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