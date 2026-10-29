using Microsoft.AspNetCore.Http.HttpResults;

using Xunit.Abstractions;

namespace giraf_core_v2.Tests.Services;

public class CitizenServiceTest : TestBase
{
    public CitizenServiceTest(ITestOutputHelper output) : base(output) { }

    private readonly int SEED_OFFSET = 123;

    [Fact]
    public async Task GetCurrentCitizen_GetsCurrentCitizen()
    {
        await using var db = CreateDbContext();

        var SeededCitizen = await CitizenSeeder.SeedAsync(db, Seed);
        var citizen = SeededCitizen.citizen;

        var service = new CitizenService(db);

        var fetchedCitizen = await service.GetCitizenAsync(citizen.Id);

        Assert.Equal(citizen.FirstName, fetchedCitizen.FirstName);
        Assert.Equal(citizen.LastName, fetchedCitizen.LastName);
        Assert.Equal(citizen.ClassId, fetchedCitizen.ClassId);
        Assert.Equal(citizen.Id, fetchedCitizen.UserId);
        Assert.Equal(citizen.Username, fetchedCitizen.Username);
    }

    [Fact]
    public async Task GetCurrentCitizen_GetsANonExistingCitizen()
    {
        await using var db = CreateDbContext();
        var service = new CitizenService(db);

        var fetchedCitizen = await service.GetCitizenAsync(1);

        Assert.Null(fetchedCitizen);
    }


    [Fact]
    public async Task UpdateCurrentCitizen_UpdatesCurrentCitizen()
    {
        await using var db = CreateDbContext();

        var SeededCitizen = await CitizenSeeder.SeedAsync(db, Seed);
        var citizen = SeededCitizen.citizen;

        // save the original user data before altering the first name
        int ogId = citizen.Id;
        string ogName = citizen.FirstName;
        string ogLastName = citizen.LastName;
        string ogUserName = citizen.Username;

        // Generate a new first name
        GetCitizenDTO faker = new Faker<GetCitizenDTO>().UseSeed(Seed + SEED_OFFSET)
            .RuleFor(u => u.FirstName, f => f.Person.FirstName);
        Assert.NotEqual(ogName, faker.FirstName);

        var service = new CitizenService(db);

        var updated = await service.PatchCitizenAsync(
            ogId,
            new UpdateCitizenDTO { FirstName = faker.FirstName });

        Assert.NotNull(updated);
        Assert.Equal(faker.FirstName, updated.FirstName);
        // Assert unchanged fields are still the same
        Assert.Equal(ogId, updated.UserId);
        Assert.Equal(ogLastName, updated.LastName);
        Assert.Equal(ogUserName, updated.Username);
    }

    [Fact]
    public async Task UpdateCurrentCitizen_UpdatesAllFields()
    {
        await using var db = CreateDbContext();

        var SeededCitizen = await CitizenSeeder.SeedAsync(db, Seed);
        var citizen = SeededCitizen.citizen;
        var newClass = await ClassSeeder.SeedAsync(db, Seed);

        // Generate new values for every field
        UpdateCitizenDTO request = new Faker<UpdateCitizenDTO>().UseSeed(Seed + SEED_OFFSET)
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
    public async Task UpdateCurrentCitizen_UpdatesANonExistingCitizen()
    {
        await using var db = CreateDbContext();
        // The class must exist so the lookup fails on the citizen, not the class
        var schoolClass = await ClassSeeder.SeedAsync(db, Seed);

        var service = new CitizenService(db);

        var updated = await service.PatchCitizenAsync(
            1,
            new UpdateCitizenDTO { ClassId = schoolClass.Id });

        // Assert that the service returns null since the citizen does not exist
        Assert.Null(updated);
    }

    [Fact]
    public async Task UpdateCurrentCitizen_UpdatesToANonExistingClass()
    {
        await using var db = CreateDbContext();

        var SeededCitizen = await CitizenSeeder.SeedAsync(db, Seed);
        var citizen = SeededCitizen.citizen;
        int ogClassId = citizen.ClassId;

        var service = new CitizenService(db);

        // No classes are seeded, so this class does not exist
        var updated = await service.PatchCitizenAsync(
            citizen.Id,
            new UpdateCitizenDTO { ClassId = ogClassId + 1 });

        // Assert that the service returns null and the citizen keeps its class
        Assert.Null(updated);
        Assert.Equal(ogClassId, citizen.ClassId);
    }

    [Fact]
    public async Task DeleteCurrentCitizen_DeletesCurrentCitizen()
    {
        await using var db = CreateDbContext();

        var SeededCitizen = await CitizenSeeder.SeedAsync(db, Seed);
        var citizen = SeededCitizen.citizen;

        var service = new CitizenService(db);
        await service.DeleteCitizenAsync(citizen.Id);
        // Make a lookup to see that the service returns null meaning the user does not exist
        Assert.Null(await service.GetCitizenAsync(citizen.Id));
    }

    [Fact]
    public async Task DeleteCurrentCitizen_DeletesANonExistingCitizen()
    {
        await using var db = CreateDbContext();
        var service = new CitizenService(db);
        // Assert that the service returns false since the user does not exist
        Assert.False(await service.DeleteCitizenAsync(1));
    }

    [Fact]
    public async Task CreateNewCitizen_CreatesNewCitizen()
    {
        await using var db = CreateDbContext();
        var schoolClass = await ClassSeeder.SeedAsync(db, Seed);

        CreateCitizenDTO request = new Faker<CreateCitizenDTO>().UseSeed(Seed + SEED_OFFSET)
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
        Assert.Null(created.Password);
        
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