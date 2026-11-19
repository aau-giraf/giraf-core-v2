using System.Net;
using System.Net.Http.Json;

namespace giraf_core_v2.Tests.Integration;

public class CitizenEndpointTest(GirafApiFactory factory, ITestOutputHelper output) : TestBase(output), IClassFixture<GirafApiFactory>
{
    private readonly UserSeeder _userSeeder = new();
    private readonly ClassSeeder _classSeeder = new();
    private readonly OrganizationSeeder _organizationSeeder = new();

    [Fact]
    public async Task GetCitizen_CitizenExists_ReturnsOk()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id, Type: UserType.Citizen));
        });
        var client = factory.CreateClient();
        var response = await client.GetAsync($"/citizens/{seededCitizen.User.Id}");

        try
        {
            response.EnsureSuccessStatusCode();
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenResponseDTO>();
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
    public async Task GetCitizen_CitizenDoesNotExist_ReturnsNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: schoolClass.Id));
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
    public async Task CreateCitizen_RequestIsValid_ReturnsCreated()
    {
        var seededClass = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            return await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        });

        CreateCitizenRequestDTO requestBody = new Faker<CreateCitizenRequestDTO>().UseSeed(Seed)
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
            var citizen = await response.Content.ReadFromJsonAsync<GetCitizenResponseDTO>();
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
    public async Task CreateCitizen_RequestIsInvalid_ReturnsBadRequest()
    {
        var seededClass = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            return await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        });

        CreateCitizenRequestDTO requestBody = new Faker<CreateCitizenRequestDTO>().UseSeed(Seed)
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
    public async Task UpdateCitizen_CitizenExists_ReturnsOk(bool isFirstNameChanged, bool isLastNameChanged, bool isClassChanged, bool isUsernameChanged)
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: schoolClass.Id));
        });
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        UpdateCitizenRequestDTO requestBody = new Faker<UpdateCitizenRequestDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => isFirstNameChanged ? f.Person.FirstName : null)
            .RuleFor(u => u.LastName, f => isLastNameChanged ? f.Person.LastName : null)
            .RuleFor(u => u.ClassId, isClassChanged ? citizen.Id : null)
            .RuleFor(u => u.Username, f => isUsernameChanged ? f.Person.UserName : null);

        var client = factory.CreateClient();

        var response = await client.PatchAsJsonAsync($"/citizens/{citizen.Id}", requestBody);

        try
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var citizenJson = await response.Content.ReadFromJsonAsync<GetCitizenResponseDTO>();
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
    public async Task UpdateCitizen_RequestIsNull_ReturnsBadRequest()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: schoolClass.Id));
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
    public async Task UpdateCitizen_CitizenDoesNotExist_ReturnsNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        });
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);
        var client = factory.CreateClient();


        UpdateCitizenRequestDTO updateCitizen = new Faker<UpdateCitizenRequestDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, citizen.Id)
            .RuleFor(u => u.Username, f => f.Person.UserName);

        var response = await client.PatchAsJsonAsync($"/citizens/{citizen.Id + 1}", updateCitizen);

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
    public async Task DeleteCitizen_CitizenExists_ReturnsNoContent()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: schoolClass.Id));
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
    public async Task DeleteCitizen_CitizenDoesNotExist_ReturnsNotFound()
    {
        var seededCitizen = await factory.ArrangeAsync(async db =>
        {
            var organization = await _organizationSeeder.SeedAsync(db, Seed);
            var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
            return await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.Citizen, ClassId: schoolClass.Id));
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