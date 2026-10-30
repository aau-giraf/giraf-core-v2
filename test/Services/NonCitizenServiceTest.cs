using System.ComponentModel.DataAnnotations;

using DotNet.Testcontainers.Configurations;

using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using Xunit.Abstractions;

public class NonCitizenServiceTest : TestBase
{
    public NonCitizenServiceTest(ITestOutputHelper output) : base(output) { }

    // Used to offset seed for different results.
    private readonly int SEED_OFFSET = 123;

    [Fact]
    public async Task GetNonCitizenById_GetsCorrectNonCitizenId()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var SeededNonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = SeededNonCitizen.NonCitizen;

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.GetNonCitizenByIdAsync(nonCitizen.Id);

        //----- ASSERT -----\\
        Assert.Equal(fetchedNonCitizen!.Id, nonCitizen.Id);
        Assert.Equal(fetchedNonCitizen!.FirstName, nonCitizen.FirstName);
        Assert.Equal(fetchedNonCitizen!.LastName, nonCitizen.LastName);
        Assert.Equal(fetchedNonCitizen!.Username, nonCitizen.Username);
        Assert.Equal(fetchedNonCitizen!.Email, nonCitizen.Email);
    }

    [Fact]
    public async Task PatchNonCitizen_PatchesCorrectNonCitizen()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var SeededNonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var nonCitizen = SeededNonCitizen.NonCitizen;

        // Save original data, because nonCitizen cannot be a constant entity.
        int ogId = nonCitizen.Id;
        string ogFirstname = nonCitizen.FirstName;
        string ogLastname = nonCitizen.LastName;
        string ogUsername = nonCitizen.Username;
        string ogEmail = nonCitizen.Email;

        // Generate different nonCitizen with another seed
        NonCitizen faker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email());


        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PatchNonCitizenAsync(
            nonCitizen.Id,
            new RequestPatchNonCitizenDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Username = faker.Username,
                Email = faker.Email
            });

        //----- ASSERT -----\\
        Assert.Equal(ogId, fetchedNonCitizen!.Id);
        Assert.Equal(faker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(faker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(faker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(faker.Email, fetchedNonCitizen!.Email);
    }

    [Fact]
    public async Task DeleteNonCitizen_DeletesCorrectNonCitizen()
    {
        await using var db = CreateDbContext();

        var SeededNonCitizens = await NonCitizenSeeder.SeedManyAsync(db, Seed, 2);
        var nonCitizen1 = SeededNonCitizens[0].NonCitizen;
        var nonCitizen2 = SeededNonCitizens[1].NonCitizen;

        Assert.NotNull(nonCitizen1);
        Assert.NotNull(nonCitizen2);

        var service = new NonCitizenService(db);

        Assert.True(await service.DeleteNonCitizenAsync(nonCitizen1.Id));

        var deletedNonCitizen = await db.NonCitizens
            .SingleOrDefaultAsync(nc => nc.Id == nonCitizen1.Id);

        Assert.Null(deletedNonCitizen);

        // Make sure the other nonCitizen was not accidentally deleted
        var remainingNonCitizens = await db.NonCitizens
            .SingleOrDefaultAsync(nc => nc.Id == nonCitizen2.Id);

        Assert.NotNull(remainingNonCitizens);
    }

    [Fact]
    public async Task PostNonCitizen_PostsCorrectNonCitizen()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var SeededNonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var nonCitizen = SeededNonCitizen.NonCitizen;

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var userRoles = await UserRoleSeeder.SeedManyAsync(db, Seed, 2);
        var userRole1 = userRoles[0];
        var userRole2 = userRoles[1];

        string password = "jdewjkfewjf21";

        // Generate different nonCitizen with another seed
        NonCitizen faker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email())
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(password, 12));


        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PostNonCitizenAsync(
            new RequestPostNonCitizenDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Username = faker.Username,
                Email = faker.Email,
                Password = faker.Password,
                Roles = [userRole1],
                OrganizationId = organization1.Id
            });

        //----- ASSERT -----\\
        Assert.Equal(faker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(faker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(faker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(faker.Email, fetchedNonCitizen!.Email);
        Assert.Equal(userRole1.Role, fetchedNonCitizen.Roles.Last().Role);
        Assert.Equal(userRole1.OrganizationId, fetchedNonCitizen.Roles.Last().OrganizationId);
        Assert.Equal(userRole1.NonCitizenId, fetchedNonCitizen.Roles.Last().NonCitizenId);

    }
}