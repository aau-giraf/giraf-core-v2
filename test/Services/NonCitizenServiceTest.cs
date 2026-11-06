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

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

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

        var SeededNoncitizen = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = SeededNoncitizen.NonCitizen;
        var ogPassword = SeededNoncitizen.PlainPassword;

        // Save original data, because nonCitizen cannot be a constant entity.
        int ogId = nonCitizen.Id;
        string ogFirstname = nonCitizen.FirstName;
        string ogLastname = nonCitizen.LastName;
        string ogUsername = nonCitizen.Username;
        string ogEmail = nonCitizen.Email;

        // New password
        string newPassword = "fjkewkf21w121";

        // Generate different nonCitizen with another seed
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => f.Internet.Email())
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(newPassword, 12));

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
        Assert.NotNull(fetchedNonCitizen.Item2);
        Assert.Equal(ogId, fetchedNonCitizen.Item2!.Id);
        Assert.Equal(nonCitizenFaker.FirstName, fetchedNonCitizen.Item2.FirstName);
        Assert.Equal(nonCitizenFaker.LastName, fetchedNonCitizen!.Item2.LastName);
        Assert.Equal(nonCitizenFaker.Username, fetchedNonCitizen!.Item2.Username);
        Assert.Equal(nonCitizenFaker.Email, fetchedNonCitizen!.Item2.Email);
        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, nonCitizenFaker.Password));
    }


    [Fact]
    public async Task DeleteNonCitizen_DeletesCorrectNonCitizen()
    {
        await using var db = CreateDbContext();

        var nonCitizensSeeder = await NonCitizenSeeder.SeedManyAsync(db, Seed, 2);
        var nonCitizen1 = nonCitizensSeeder[0].NonCitizen;
        var nonCitizen2 = nonCitizensSeeder[1].NonCitizen;

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
        Assert.NotNull(fetchedNonCitizen.Item2);
        Assert.Equal(nonCitizenFaker.FirstName, fetchedNonCitizen!.Item2!.FirstName);
        Assert.Equal(nonCitizenFaker.LastName, fetchedNonCitizen!.Item2!.LastName);
        Assert.Equal(nonCitizenFaker.Username, fetchedNonCitizen!.Item2!.Username);
        Assert.Equal(nonCitizenFaker.Email, fetchedNonCitizen!.Item2!.Email);
    }

    // ---------- /userRoles ---------- \\

    [Fact]
    public async Task GetUserRoles_GetsCorrectUserRoles()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

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
            var roleString = userRole.Role.ToString();
            var orgString = userRole.OrganizationId.ToString();
            var roleOrgString = roleString + "|" + orgString;
            Assert.Contains(roleOrgString, fetchedUserRoles.RoleOrganizationTuples);
        }
    }

    [Fact]
    public async Task DeleteUserRole_DeletesCorrectUserRole()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        var organizations = await OrganizationSeeder.SeedManyAsync(db, Seed, 3);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        var organization3 = organizations[2];

        var userRoles = await UserRoleSeeder.SeedManyAsync(db, Seed, [(organization1.Id, nonCitizen.Id), (organization2.Id, nonCitizen.Id), (organization3.Id, nonCitizen.Id)]);
        var userRole1 = userRoles[0];
        var userRole2 = userRoles[1];
        var userRole3 = userRoles[2];

        List<(RoleType, int)> ogUserRoles = new List<(RoleType, int)>();

        foreach (var userRole in userRoles)
        {
            ogUserRoles.Add((userRole.Role, userRole.OrganizationId));
        }

        var service = new NonCitizenService(db);

        Assert.True(await service.DeleteUserRoleAsync(nonCitizen.Id, new RequestNonCitizenUserRoleDTO
        {
            Role = userRole1.Role,
            OrganizationId = userRole1.OrganizationId
        }));

        var deletedUserRole = await db.UserRoles
            .SingleOrDefaultAsync(ur => ur.NonCitizenId == nonCitizen.Id && ur.Role == userRole1.Role && ur.OrganizationId == userRole1.OrganizationId);

        Assert.Null(deletedUserRole);

        // Make sure the other userRoles was not accidentally deleted
        var remainingUserRoles = await db.UserRoles.ToListAsync();
        Assert.Equal(2, remainingUserRoles.Count);
        Assert.NotNull(remainingUserRoles);
        foreach (var remainingUserRole in remainingUserRoles)
        {
            Assert.Contains((remainingUserRole.Role, remainingUserRole.OrganizationId), ogUserRoles);
        }
    }


    [Fact]
    public async Task PostUserRole_PostsCorrectUserRole()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

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
            Role = userRoleFaker.Role,
            OrganizationId = userRoleFaker.OrganizationId
        });

        //----- ASSERT -----\\  
        Assert.NotNull(fetchedUserRoles.Item2);
        Assert.Equal(3, fetchedUserRoles.Item2!.RoleOrganizationTuples.Count);

        var roleString = userRoleFaker.Role.ToString();
        var orgString = userRoleFaker.OrganizationId.ToString();
        var roleOrgString = roleString + "|" + orgString;

        Assert.Contains(roleOrgString, fetchedUserRoles.Item2.RoleOrganizationTuples);
        foreach (var userRole in userRoles)
        {
            var roleString1 = userRole.Role.ToString();
            var orgString1 = userRole.OrganizationId.ToString();
            var roleOrgString1 = roleString + "|" + orgString;
            Assert.Contains(roleOrgString1, fetchedUserRoles.Item2.RoleOrganizationTuples);
        }
    }

    // ---------- /guardianships ---------- \\

    [Fact]
    public async Task GetGuardianships_GetsCorrectguardianships()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        var citizens = await CitizenSeeder.SeedManyAsync(db, Seed, 2);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];

        var guardianships = await GuardianshipSeeder.SeedManyAsync(db, Seed, [(citizen1.citizen.Id, nonCitizen.Id), (citizen2.citizen.Id, nonCitizen.Id)]);
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

    [Fact]
    public async Task DeleteGuardianship_DeletesCorrectGuardianship()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        var citizens = await CitizenSeeder.SeedManyAsync(db, Seed, 3);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];
        var citizen3 = citizens[2];

        var guardianships = await GuardianshipSeeder.SeedManyAsync(db, Seed, [(citizen1.citizen.Id, nonCitizen.Id), (citizen2.citizen.Id, nonCitizen.Id), (citizen3.citizen.Id, nonCitizen.Id)]);
        var guardianship1 = guardianships[0];
        var guardianship2 = guardianships[1];
        var guardianship3 = guardianships[2];

        List<(int, int)> ogGuardianships = new List<(int, int)>();

        foreach (var guardianship in guardianships)
        {
            ogGuardianships.Add((guardianship.CitizenId, guardianship.GuardianId));
        }

        // Format guardianship to Request format
        (int, int) citizenNonCitizenTuple = (guardianship1.CitizenId, guardianship1.GuardianId);

        var service = new NonCitizenService(db);

        Assert.True(await service.DeleteGuardianshipAsync(nonCitizen.Id, new RequestGuardianshipDTO
        {
            CitizenId = citizenNonCitizenTuple.Item1
        }));

        var deletedGuardianship = await db.Guardianships
            .SingleOrDefaultAsync(gs => gs.CitizenId == citizenNonCitizenTuple.Item1 && gs.GuardianId == citizenNonCitizenTuple.Item2);

        Assert.Null(deletedGuardianship);

        // Make sure the other userRoles was not accidentally deleted
        var remainingGuardianships = await db.Guardianships.ToListAsync();
        Assert.Equal(2, remainingGuardianships.Count);
        Assert.NotNull(remainingGuardianships);
        foreach (var remainingGuardianship in remainingGuardianships)
        {
            Assert.Contains((remainingGuardianship.CitizenId, remainingGuardianship.GuardianId), ogGuardianships);
        }
    }

    [Fact]
    public async Task PostGuardianship_PostsGuardianship()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        var citizens = await CitizenSeeder.SeedManyAsync(db, Seed, 3);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];
        var citizen3 = citizens[2];

        var guardianships = await GuardianshipSeeder.SeedManyAsync(db, Seed, [(citizen1.citizen.Id, nonCitizen.Id), (citizen2.citizen.Id, nonCitizen.Id)]);

        // Faker guardianship
        Guardianship guardianshipFaker = new Faker<Guardianship>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(gs => gs.GuardianId, f => nonCitizen.Id)
                                    .RuleFor(gs => gs.CitizenId, f => citizen3.citizen.Id);

        //----- ACT -----\\              
        var service = new NonCitizenService(db);

        var fetchedGuardianships = await service.PostGuardianshipAsync(nonCitizen.Id, new RequestGuardianshipDTO
        {
            CitizenId = citizen3.citizen.Id
        });

        //----- ASSERT -----\\  
        Assert.NotNull(fetchedGuardianships.Item2);
        Assert.Equal(3, fetchedGuardianships.Item2!.CitizenIds.Count);
        Assert.Contains(guardianshipFaker.CitizenId, fetchedGuardianships.Item2.CitizenIds);
        foreach (var guardianship in guardianships)
        {
            Assert.Contains(guardianship.CitizenId, fetchedGuardianships.Item2.CitizenIds);
        }
    }

    // ---------- /nonCitizens on invalid inputs ---------- \\
    [Fact]
    public async Task PatchNonCitizen_ReturnsUnauthorizedResponseOnNonUnauthorizedOldPassword()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var SeededNoncitizen = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = SeededNoncitizen.NonCitizen;
        var ogPassword = SeededNoncitizen.PlainPassword;

        // New password
        string newPassword = "fjkewkf21w121";

        // Generate different nonCitizen with another seed
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(newPassword, 12));

        var service = new NonCitizenService(db);

        //----- ACT -----\\
        var fetchedNonCitizen = await service.PatchNonCitizenAsync(
            nonCitizen.Id,
            new RequestPatchNonCitizenDTO
            {
                OldPassword = ogPassword + "udsw",
                NewPassword = nonCitizenFaker.Password,
            });

        //----- ASSERT -----\\
        Assert.Null(fetchedNonCitizen.Item2);
        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, nonCitizenFaker.Password));
        Assert.Equal(HttpReponseType.Unauthorized.ToString(), fetchedNonCitizen.Item1.ToString());
    }

    [Fact]
    public async Task PostNonCitizen_ReturnsConflictResponseOnNonUniqueEmail()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var SeededNoncitizen = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = SeededNoncitizen.NonCitizen;
        var ogPassword = SeededNoncitizen.PlainPassword;

        var service = new NonCitizenService(db);

        // Faker nonCitizen
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
                                    .RuleFor(nc => nc.LastName, f => f.Name.LastName())
                                    .RuleFor(nc => nc.Username, f => f.Internet.UserName())
                                    .RuleFor(nc => nc.Email, f => nonCitizen.Email)
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
        Assert.Null(fetchedNonCitizen.Item2);
        Assert.Equal(HttpReponseType.Conflict.ToString(), fetchedNonCitizen.Item1.ToString());
    }

    // ---------- /nonCitizens/userRoles on invalid inputs ---------- \\

    [Fact]
    public async Task PostUserRole_RetunsNotFoundOnNonExistingOrganisation()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        // No organizations yet -> So new userRole is forced to fail.

        // Faker userRole
        UserRole userRoleFaker = new Faker<UserRole>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(urf => urf.NonCitizenId, f => nonCitizen.Id)
                                    .RuleFor(urf => urf.Role, f => f.PickRandom<RoleType>())
                                    .RuleFor(urf => urf.OrganizationId, f => f.Random.Int(1, 1000));

        //----- ACT -----\\              
        var service = new NonCitizenService(db);

        var fetchedUserRoles = await service.PostUserRoleAsync(nonCitizen.Id, new RequestNonCitizenUserRoleDTO
        {
            Role = userRoleFaker.Role,
            OrganizationId = userRoleFaker.OrganizationId
        });

        //----- ASSERT -----\\  
        Assert.Null(fetchedUserRoles.Item2);
        Assert.Equal(HttpReponseType.NotFound.ToString(), fetchedUserRoles.Item1.ToString());
    }

    // ---------- /nonCitizens/guardianships on invalid inputs ---------- \\

    [Fact]
    public async Task PostGuardianship_ReturnsConflictResonseOnExistingGuardianship()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        var nonCitizenSeeder = await NonCitizenSeeder.SeedAsync(db, Seed);
        var nonCitizen = nonCitizenSeeder.NonCitizen;

        var citizens = await CitizenSeeder.SeedManyAsync(db, Seed, 3);
        var citizen1 = citizens[0];
        var citizen2 = citizens[1];
        var citizen3 = citizens[2];

        var guardianships = await GuardianshipSeeder.SeedManyAsync(db, Seed, [(citizen1.citizen.Id, nonCitizen.Id), (citizen2.citizen.Id, nonCitizen.Id)]);

        // Faker guardianship
        Guardianship guardianshipFaker = new Faker<Guardianship>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(gs => gs.GuardianId, f => nonCitizen.Id)
                                    .RuleFor(gs => gs.CitizenId, f => citizen2.citizen.Id);

        //----- ACT -----\\              
        var service = new NonCitizenService(db);

        var fetchedGuardianships = await service.PostGuardianshipAsync(nonCitizen.Id, new RequestGuardianshipDTO
        {
            CitizenId = citizen2.citizen.Id
        });

        //----- ASSERT -----\\  
        Assert.Null(fetchedGuardianships.Item2);
        Assert.Equal(HttpReponseType.Conflict.ToString(), fetchedGuardianships.Item1.ToString());
    }

}