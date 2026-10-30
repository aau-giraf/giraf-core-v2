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

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

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

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        // Save original data, because nonCitizen cannot be a constant entity.
        int ogId = nonCitizen.Id;
        string ogFirstname = nonCitizen.FirstName;
        string ogLastname = nonCitizen.LastName;
        string ogUsername = nonCitizen.Username;
        string ogEmail = nonCitizen.Email;
        string ogPassword = nonCitizen.Password;

        // New password
        string Password = "fjkewkf21w121";

        // Generate different nonCitizen with another seed
        NonCitizen faker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email())
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(Password, 12));

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PatchNonCitizenAsync(
            nonCitizen.Id,
            new RequestPatchNonCitizenDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Username = faker.Username,
                Email = faker.Email,
                OldPassword = ogPassword,
                NewPassword = faker.Password
            });

        //----- ASSERT -----\\
        Assert.Equal(ogId, fetchedNonCitizen!.Id);
        Assert.Equal(faker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(faker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(faker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(faker.Email, fetchedNonCitizen!.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(Password, faker.Password));
    }

    [Fact]
    public async Task DeleteNonCitizen_DeletesCorrectNonCitizen()
    {
        await using var db = CreateDbContext();

        var nonCitizens = await NonCitizenSeeder.SeedManyAsync(db, Seed, 2);
        var nonCitizen1 = nonCitizens[0];
        var nonCitizen2 = nonCitizens[1];

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

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var service = new NonCitizenService(db);

        // Faker nonCitizen
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email())
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12));

        // Faker userRole
        UserRole userRoleFaker = new Faker<UserRole>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(ur => ur.OrganizationId, f => organization1.Id)
                                    .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PostNonCitizenAsync(
            new RequestPostNonCitizenDTO
            {
                FirstName = nonCitizenFaker.FirstName,
                LastName = nonCitizenFaker.LastName,
                Username = nonCitizenFaker.Username,
                Email = nonCitizenFaker.Email,
                Password = nonCitizenFaker.Password,
                RoleOrganizationTuples = [(userRoleFaker.Role, userRoleFaker.OrganizationId)],
            });

        //----- ASSERT -----\\
        Assert.NotNull(fetchedNonCitizen);
        Assert.Equal(nonCitizenFaker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(nonCitizenFaker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(nonCitizenFaker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(nonCitizenFaker.Email, fetchedNonCitizen!.Email);
        Assert.Equal(userRoleFaker.Role, fetchedNonCitizen.Roles.Last().Role);
        Assert.Equal(userRoleFaker.OrganizationId, fetchedNonCitizen.Roles.Last().OrganizationId);
    }
}