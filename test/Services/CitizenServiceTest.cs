namespace giraf_core_v2.Tests.Services;

public class CitizenServiceTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly UserSeeder _userSeeder = new();
    private readonly OrganizationSeeder _organizationSeeder = new();
    private readonly ClassSeeder _classSeeder = new();

    [Fact]
    public async Task GetCitizen_CitizenExists_ReturnsCitizen()
    {
        await using var db = CreateDbContext();

        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var seededCitizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        var service = new CitizenService(db);

        var fetchedCitizen = await service.GetCitizenAsync(citizen.Id);

        Assert.NotNull(fetchedCitizen);
        Assert.Equal(citizen.FirstName, fetchedCitizen.FirstName);
        Assert.Equal(citizen.LastName, fetchedCitizen.LastName);
        Assert.Equal(citizen.ClassId, fetchedCitizen.ClassId);
        Assert.Equal(citizen.Id, fetchedCitizen.UserId);
        Assert.Equal(citizen.Username, fetchedCitizen.Username);
    }

    [Fact]
    public async Task GetCitizen_CitizenDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var service = new CitizenService(db);

        var fetchedCitizen = await service.GetCitizenAsync(1);

        Assert.Null(fetchedCitizen);
    }


    [Fact]
    public async Task PatchCitizen_CitizenExists_UpdatesCitizen()
    {
        await using var db = CreateDbContext();

        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var seededCitizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        // save the original user data before altering the first name
        int originalUserId = citizen.Id;
        string ogName = citizen.FirstName;
        string ogLastName = citizen.LastName;
        string ogUserName = citizen.Username;

        // Generate a new first name
        GetCitizenResponseDTO faker = new Faker<GetCitizenResponseDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName);
        Assert.NotEqual(ogName, faker.FirstName);

        var service = new CitizenService(db);

        var updated = await service.PatchCitizenAsync(
            originalUserId,
            new UpdateCitizenRequestDTO { FirstName = faker.FirstName });

        Assert.NotNull(updated);
        Assert.Equal(faker.FirstName, updated.FirstName);
        // Assert unchanged fields are still the same
        Assert.Equal(originalUserId, updated.UserId);
        Assert.Equal(ogLastName, updated.LastName);
        Assert.Equal(ogUserName, updated.Username);
    }

    [Fact]
    public async Task PatchCitizen_AllFieldsProvided_UpdatesAllFields()
    {
        await using var db = CreateDbContext();
        // Create a random organization the class has to exist in
        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var seededCitizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        var newClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        // Generate new values for every field
        UpdateCitizenRequestDTO request = new Faker<UpdateCitizenRequestDTO>().UseSeed(Seed)
            .RuleFor(u => u.ClassId, newClass.Id)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.Username, f => f.Person.UserName);

        var service = new CitizenService(db);

        var updated = await service.PatchCitizenAsync(citizen.Id, request);

        // Assert the returned DTO
        Assert.NotNull(updated);
        Assert.Equal(citizen.Id, updated.UserId);
        Assert.Equal(newClass.Id, updated.ClassId);
        Assert.Equal(request.FirstName, updated.FirstName);
        Assert.Equal(request.LastName, updated.LastName);
        Assert.Equal(request.Username, updated.Username);

        // Assert the changes were saved
        var saved = await db.Citizens.SingleAsync(c => c.Id == citizen.Id);
        Assert.Equal(newClass.Id, saved.ClassId);
        Assert.Equal(request.FirstName, saved.FirstName);
        Assert.Equal(request.LastName, saved.LastName);
        Assert.Equal(request.Username, saved.Username);
    }

    [Fact]
    public async Task PatchCitizen_CitizenDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();
        // Create a random organization the class has to exist in
        var organization = await _organizationSeeder.SeedAsync(db, Seed);

        // The class must exist so the lookup fails on the citizen, not the class
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));

        var service = new CitizenService(db);

        var updated = await service.PatchCitizenAsync(
            1,
            new UpdateCitizenRequestDTO { ClassId = schoolClass.Id });

        // Assert that the service returns null since the citizen does not exist
        Assert.Null(updated);
    }

    [Fact]
    public async Task PatchCitizen_ClassDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var seededCitizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);
        int ogClassId = citizen.ClassId;

        var service = new CitizenService(db);

        // No classes are seeded, so this class does not exist
        var updated = await service.PatchCitizenAsync(
            citizen.Id,
            new UpdateCitizenRequestDTO { ClassId = ogClassId + 1 });

        // Assert that the service returns null and the citizen keeps its class
        Assert.Null(updated);
        Assert.Equal(ogClassId, citizen.ClassId);
    }

    [Fact]
    public async Task DeleteCitizen_CitizenExists_DeletesCitizen()
    {
        await using var db = CreateDbContext();

        // Create a bogus org and class the citizen is being created in
        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));
        var seededCitizen = await _userSeeder.SeedAsync(db, Seed, new(ClassId: schoolClass.Id));
        var citizen = Assert.IsType<Citizen>(seededCitizen.User);

        var service = new CitizenService(db);
        await service.DeleteCitizenAsync(citizen.Id);
        // Make a lookup to see that the service returns null meaning the user does not exist
        Assert.Null(await service.GetCitizenAsync(citizen.Id));
    }

    [Fact]
    public async Task DeleteCitizen_CitizenDoesNotExist_ReturnsFalse()
    {
        await using var db = CreateDbContext();
        var service = new CitizenService(db);
        // Assert that the service returns false since the user does not exist
        Assert.False(await service.DeleteCitizenAsync(1));
    }

    [Fact]
    public async Task CreateCitizen_ClassExists_CreatesCitizen()
    {
        await using var db = CreateDbContext();

        // Create a random organization the class has to exist in
        var organization = await _organizationSeeder.SeedAsync(db, Seed);
        var schoolClass = await _classSeeder.SeedAsync(db, Seed, new(OrganizationId: organization.Id));

        CreateCitizenRequestDTO request = new Faker<CreateCitizenRequestDTO>().UseSeed(Seed)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName)
            .RuleFor(u => u.LastName, f => f.Person.LastName)
            .RuleFor(u => u.ClassId, schoolClass.Id)
            .RuleFor(u => u.Username, f => f.Person.UserName)
            .RuleFor(u => u.Password, f => f.Random.Word());

        var service = new CitizenService(db);
        var created = await service.CreateCitizenAsync(request);

        // Assert the returned DTO
        Assert.NotNull(created);
        Assert.Equal(request.FirstName, created.FirstName);
        Assert.Equal(request.LastName, created.LastName);
        Assert.Equal(request.Username, created.Username);
        Assert.Equal(request.ClassId, created.ClassId);

        // Assert the citizen was actually saved
        var saved = await db.Citizens.SingleAsync(c => c.Username == request.Username);
        Assert.Equal(request.FirstName, saved.FirstName);
        Assert.Equal(request.LastName, saved.LastName);
        Assert.Equal(schoolClass.Id, saved.ClassId);

        // Assert the password was hashed, not stored as plain text
        Assert.NotEqual(request.Password, saved.Password);
        Assert.True(BCrypt.Net.BCrypt.Verify(request.Password, saved.Password));
    }
}