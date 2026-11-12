public class NonCitizenServiceTest(ITestOutputHelper output) : TestBase(output)
{

    // Used to offset seed for different results.
    private readonly int _seedOffset = 123;

    private readonly UserSeeder _userSeeder = new();
    private readonly OrganizationSeeder _orgSeeder = new();
    private readonly UserRoleSeeder _userRoleSeeder = new();
    private readonly GuardianshipSeeder _guardianshipSeeder = new();

    // ---------- /nonCitizens ---------- \\

    [Fact]
    public async Task GetNonCitizenById_GetsCorrectNonCitizenId()
    {
        //----- ARRANGE -----\\
        await using var db = CreateDbContext();

        UserOptions opt = new(Type: UserType.NonCitizen);
        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, opt);
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User); ;

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

        UserOptions opt = new(Type: UserType.NonCitizen);
        var seededNonCitizen = await _userSeeder.SeedAsync(db, Seed, opt);
        var nonCitizen = Assert.IsType<NonCitizen>(seededNonCitizen.User);
        var ogPassword = seededNonCitizen.PlainPassword;

        // Save original data, because nonCitizen cannot be a constant entity.
        int ogId = nonCitizen.Id;
        string ogFirstname = nonCitizen.FirstName;
        string ogLastname = nonCitizen.LastName;
        string ogUsername = nonCitizen.Username;
        string ogEmail = nonCitizen.Email;

        // New password
        string newPassword = "fjkewkf21w121";

        // Generate different nonCitizen with another seed
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + _seedOffset)
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

        List<UserOptions> opt = [new(Type: UserType.NonCitizen), new(Type: UserType.NonCitizen)];
        var nonCitizensSeeder = await _userSeeder.SeedManyAsync(db, Seed, 2, opt);
        var nonCitizen1 = Assert.IsType<NonCitizen>(nonCitizensSeeder[0].User);
        var nonCitizen2 = Assert.IsType<NonCitizen>(nonCitizensSeeder[1].User);

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
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + _seedOffset)
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

        UserOptions userOpt = new(Type: UserType.NonCitizen);
        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, userOpt);
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);

        var organizations = await _orgSeeder.SeedManyAsync(db, Seed, 2);
        var organization1 = organizations[0];
        var organization2 = organizations[1];

        List<UserRoleOptions> urOpt = [new(organization1.Id, nonCitizen.Id), new(organization2.Id, nonCitizen.Id)];
        var userRoles = await _userRoleSeeder.SeedManyAsync(db, Seed, 2, urOpt);

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

        UserOptions userOptions = new(Type: UserType.NonCitizen);
        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, userOptions);
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);

        var organizations = await _orgSeeder.SeedManyAsync(db, Seed, 3);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        var organization3 = organizations[2];

        List<UserRoleOptions> urOpt = [new(organization1.Id, nonCitizen.Id), new(organization2.Id, nonCitizen.Id), new(organization3.Id, nonCitizen.Id)];
        var userRoles = await _userRoleSeeder.SeedManyAsync(db, Seed, 3, urOpt);
        var userRole1 = userRoles[0];
        var userRole2 = userRoles[1];
        var userRole3 = userRoles[2];

        List<(RoleType, int)> ogUserRoles = [];

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

        UserOptions uOpt = new(Type: UserType.NonCitizen);
        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, uOpt);
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);

        var organizations = await _orgSeeder.SeedManyAsync(db, Seed, 3);
        var organization1 = organizations[0];
        var organization2 = organizations[1];
        var organization3 = organizations[2];

        List<UserRoleOptions> urOpt = [new(organization1.Id, nonCitizen.Id), new(organization2.Id, nonCitizen.Id)];
        var userRoles = await _userRoleSeeder.SeedManyAsync(db, Seed, 3, urOpt);

        // Faker userRole
        UserRole userRoleFaker = new Faker<UserRole>().UseSeed(Seed + _seedOffset)
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

        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);


        var citizen1 = await _userSeeder.SeedAsync(db, Seed);
        var citizen2 = await _userSeeder.SeedAsync(db, Seed);

        List<GuardianshipOptions> guardOpt = [new(citizen1.User.Id, nonCitizen.Id), new(citizen2.User.Id, nonCitizen.Id)];
        var guardianships = await _guardianshipSeeder.SeedManyAsync(db, Seed, 3, guardOpt);
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

        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);

        var citizen1 = await _userSeeder.SeedAsync(db, Seed);
        var citizen2 = await _userSeeder.SeedAsync(db, Seed);
        var citizen3 = await _userSeeder.SeedAsync(db, Seed);


        List<GuardianshipOptions> opt = [new(citizen1.User.Id, nonCitizen.Id), new(citizen2.User.Id, nonCitizen.Id), new(citizen3.User.Id, nonCitizen.Id)];
        var guardianships = await _guardianshipSeeder.SeedManyAsync(db, Seed, 3, opt);
        var guardianship1 = guardianships[0];
        var guardianship2 = guardianships[1];
        var guardianship3 = guardianships[2];

        List<(int, int)> ogGuardianships = [];

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

        var nonCitizenSeeder = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(nonCitizenSeeder.User);

        var citizen1 = await _userSeeder.SeedAsync(db, Seed);
        var citizen2 = await _userSeeder.SeedAsync(db, Seed);
        var citizen3 = await _userSeeder.SeedAsync(db, Seed);

        List<GuardianshipOptions> opt = [new(citizen1.User.Id, nonCitizen.Id), new(citizen2.User.Id, nonCitizen.Id)];
        var guardianships = await _guardianshipSeeder.SeedManyAsync(db, Seed, 2, opt);

        // Faker guardianship
        Guardianship guardianshipFaker = new Faker<Guardianship>().UseSeed(Seed + _seedOffset)
                                    .RuleFor(gs => gs.GuardianId, f => nonCitizen.Id)
                                    .RuleFor(gs => gs.CitizenId, f => citizen3.User.Id);

        //----- ACT -----\\
        var service = new NonCitizenService(db);

        var fetchedGuardianships = await service.PostGuardianshipAsync(nonCitizen.Id, new RequestGuardianshipDTO
        {
            CitizenId = citizen3.User.Id
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

        var seededNoncitizen = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(seededNoncitizen.User);
        var ogPassword = seededNoncitizen.PlainPassword;

        // New password
        string newPassword = "fjkewkf21w121";

        // Generate different nonCitizen with another seed
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + _seedOffset)
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

        var seededNoncitizen = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(seededNoncitizen.User);
        var ogPassword = seededNoncitizen.PlainPassword;

        var service = new NonCitizenService(db);

        // Faker nonCitizen
        NonCitizen nonCitizenFaker = new Faker<NonCitizen>().UseSeed(Seed + _seedOffset)
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

        var seededNoncitizen = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(seededNoncitizen.User);

        // No organizations yet -> So new userRole is forced to fail.

        // Faker userRole
        UserRole userRoleFaker = new Faker<UserRole>().UseSeed(Seed + _seedOffset)
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

        var seededNoncitizen = await _userSeeder.SeedAsync(db, Seed, new(Type: UserType.NonCitizen));
        var nonCitizen = Assert.IsType<NonCitizen>(seededNoncitizen.User);

        var citizen1 = await _userSeeder.SeedAsync(db, Seed);
        var citizen2 = await _userSeeder.SeedAsync(db, Seed);
        var citizen3 = await _userSeeder.SeedAsync(db, Seed);

        List<GuardianshipOptions> opt = [new(citizen1.User.Id, nonCitizen.Id), new(citizen2.User.Id, nonCitizen.Id)];
        var guardianships = await _guardianshipSeeder.SeedManyAsync(db, Seed, 2, opt);

        // Faker guardianship
        Guardianship guardianshipFaker = new Faker<Guardianship>().UseSeed(Seed + _seedOffset)
                                    .RuleFor(gs => gs.GuardianId, f => nonCitizen.Id)
                                    .RuleFor(gs => gs.CitizenId, f => citizen2.User.Id);

        //----- ACT -----\\
        var service = new NonCitizenService(db);

        var fetchedGuardianships = await service.PostGuardianshipAsync(nonCitizen.Id, new RequestGuardianshipDTO
        {
            CitizenId = citizen2.User.Id
        });

        //----- ASSERT -----\\
        Assert.Null(fetchedGuardianships.Item2);
        Assert.Equal(HttpReponseType.Conflict.ToString(), fetchedGuardianships.Item1.ToString());
    }

}