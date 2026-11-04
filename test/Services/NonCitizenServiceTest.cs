using System.ComponentModel.DataAnnotations;

using Bogus.DataSets;

using DotNet.Testcontainers.Configurations;

using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

using Xunit.Abstractions;

public class NonCitizenServiceTest : TestBase
{
    public NonCitizenServiceTest(ITestOutputHelper output) : base(output) { }

    // Used to offset seed for different results.
    private readonly int SEED_OFFSET = 123;

    // ---------- /nonCitizens ---------- \\

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
        Assert.Equal(nonCitizen.Id, fetchedNonCitizen!.Id);
        Assert.Equal(nonCitizen.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(nonCitizen.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(nonCitizen.Username, fetchedNonCitizen!.Username);
        Assert.Equal(nonCitizen.Email, fetchedNonCitizen!.Email);
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
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => nonCitizen.Email)
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(Password, 12));

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PatchNonCitizenAsync(
            nonCitizen.Id,
            new RequestPatchNonCitizenDTO
            {
                FirstName = nonCitizenFaker.FirstName,
                LastName = nonCitizenFaker.LastName,
                Username = nonCitizenFaker.Username,
                Email = nonCitizenFaker.Email,
                OldPassword = ogPassword,
                NewPassword = nonCitizenFaker.Password,
            });

        //----- ASSERT -----\\
        Assert.Equal(ogId, fetchedNonCitizen!.Id);
        Assert.Equal(nonCitizenFaker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(nonCitizenFaker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(nonCitizenFaker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(nonCitizenFaker.Email, fetchedNonCitizen!.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(Password, nonCitizenFaker.Password));
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

        var service = new NonCitizenService(db);

        // Faker nonCitizen
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email())
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12));

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PostNonCitizenAsync(
            new RequestPostNonCitizenDTO
            {
                FirstName = nonCitizenFaker.FirstName,
                LastName = nonCitizenFaker.LastName,
                Username = nonCitizenFaker.Username,
                Email = nonCitizenFaker.Email,
                Password = nonCitizenFaker.Password,
            });

        //----- ASSERT -----\\
        Assert.NotNull(fetchedNonCitizen);
        Assert.Equal(nonCitizenFaker.FirstName, fetchedNonCitizen!.FirstName);
        Assert.Equal(nonCitizenFaker.LastName, fetchedNonCitizen!.LastName);
        Assert.Equal(nonCitizenFaker.Username, fetchedNonCitizen!.Username);
        Assert.Equal(nonCitizenFaker.Email, fetchedNonCitizen!.Email);
    }

    // ---------- /userRoles ---------- \\

    [Fact]
    public async Task GetUserRoles_GetsCorrectUserRoles()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        var userRoles = await UserRoleSeeder.SeedManyAsync(db, Seed, [(organization1.Id, nonCitizen.Id), (organization2.Id, nonCitizen.Id)]);

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedUserRoles = await service.GetUserRolesAsync(nonCitizen.Id);

        //----- ASSERT -----\\
        Assert.Equal(nonCitizen.Roles.Count, fetchedUserRoles!.RoleOrganizationTuples.Count);
        foreach (var userRole in nonCitizen.Roles)
        {
            Assert.Contains((userRole.Role, userRole.OrganizationId), fetchedUserRoles.RoleOrganizationTuples);
        }
    }

    [Fact]
    public async Task DeleteUserRole_DeletesCorrectUserRole()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 3);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        var organization3 = organizations[2];

        var userRoles = await UserRoleSeeder.SeedManyAsync(db, Seed, [(organization1.Id, nonCitizen.Id), (organization2.Id, nonCitizen.Id), (organization3.Id, nonCitizen.Id)]);

        List<(RoleType, int)> ogUserRoles = new List<(RoleType, int)>();

        foreach (var userRole in userRoles)
        {
            ogUserRoles.Add((userRole.Role, userRole.OrganizationId));
        }

        // Format useRole to Request format
        (RoleType, int) roleOrganizationTuple = (userRoles.First().Role, userRoles.First().OrganizationId);

        var service = new NonCitizenService(db);

        Assert.True(await service.DeleteUserRoleAsync(nonCitizen.Id, new RequestNonCitizenUserRoleDTO
        {
            RoleOrganizationTuple = roleOrganizationTuple
        }));

        var deletedUserRole = await db.UserRoles
            .SingleOrDefaultAsync(ur => ur.NonCitizenId == nonCitizen.Id && ur.Role == roleOrganizationTuple.Item1 && ur.OrganizationId == roleOrganizationTuple.Item2);

        Assert.Null(deletedUserRole);

        // Make sure the other userRoles was not accidentally deleted
        var remainingUserRoles = await db.UserRoles.ToListAsync();
        Assert.Equal(2, remainingUserRoles.Count);
        Assert.NotNull(remainingUserRoles);
    }


    [Fact]
    public async Task PostUserRole_PostsCorrectUserRole()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 3);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        var organization3 = organizations[2];

        var userRoles = await UserRoleSeeder.SeedManyAsync(db, Seed, [(organization1.Id, nonCitizen.Id), (organization2.Id, nonCitizen.Id)]);

        // Faker userRole
        UserRole userRoleFaker = new Faker<UserRole>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(urf => urf.NonCitizenId, f => nonCitizen.Id)
                                    .RuleFor(urf => urf.Role, f => f.PickRandom<RoleType>())
                                    .RuleFor(urf => urf.OrganizationId, organization3.Id);

        //----- ACT -----\\              
        var service = new NonCitizenService(db);

        var fetchedUserRoles = await service.PostUserRoleAsync(nonCitizen.Id, new RequestNonCitizenUserRoleDTO
        {
            RoleOrganizationTuple = (userRoleFaker.Role, userRoleFaker.OrganizationId)
        });

        //----- ASSERT -----\\  
        Assert.NotNull(fetchedUserRoles);
        Assert.Equal(3, fetchedUserRoles.RoleOrganizationTuples.Count);
        Assert.Contains((userRoleFaker.Role, userRoleFaker.OrganizationId), fetchedUserRoles.RoleOrganizationTuples);

    }

    // ---------- /guardianships ---------- \\

    [Fact]
    public async Task GetGuardianships_GetsCorrectguardianships()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizen = await NonCitizenSeeder.SeedAsync(db, Seed);

        var citizens = await CitizenSeeder.SeedManyAsync(db, Seed, 2);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];

        var guardianships = await GuardianshipSeeder.SeedManyAsync(db, Seed, [(nonCitizen.Id, citizen1.citizen.Id), (nonCitizen.Id, citizen2.citizen.Id)]);
        var guardianship1 = guardianships[0];
        var guardianship2 = guardianships[1];

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedGuardianships = await service.GetGuardianshipsAsync(nonCitizen.Id);

        //----- ASSERT -----\\
        Assert.Equal(nonCitizen.Guardianships.Count, fetchedGuardianships!.CitizenIds.Count);
        foreach (var guardianship in nonCitizen.Guardianships)
        {
            Assert.Contains(guardianship.CitizenId, fetchedGuardianships.CitizenIds);
        }
    }

}