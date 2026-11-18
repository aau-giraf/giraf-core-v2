using Microsoft.IdentityModel.JsonWebTokens;

using Xunit.Abstractions;

public class UserServiceTest : TestBase
{
    public UserServiceTest(ITestOutputHelper output) : base(output) { }

    // used to offset seed for different results.
    private readonly int SEED_OFFSET = 123;

    // TODO: add sanitization tests, once sanitization has been made.
    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesTheUser()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);

        var user = SeededUser.User;

        // save origianl data, because user cannot be a constant entity.
        int ogId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogUsername = user.Username;

        // generate a different firstname with another seed
        User faker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(u => u.FirstName, f => f.Name.FirstName());


        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserDTO
            {
                FirstName = faker.FirstName
            });

        Assert.Equal(ogId, user!.Id);
        Assert.Equal(faker.FirstName, user!.FirstName);
        Assert.Equal(ogLastname, user!.LastName);
        Assert.Equal(ogUsername, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUserAsync_DoesNotUpdateNonExistingUser()
    {
        await using var db = CreateDbContext();

        int id = 1;


        var service = new UserService(db);

        var user = await service.UpdateCurrentUserAsync(
            id,
            new UpdateUserDTO
            {
                FirstName = ":)"
            });

        Assert.Null(user);

    }

    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesEntireUser()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);

        var user = SeededUser.User;

        // save origianl data, because user cannot be a constant entity.
        int ogId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogUsername = user.Username;

        // generate different user with another seed
        User faker = new Faker<NonCitizen>().UseSeed(Seed + SEED_OFFSET)
                                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                                    .RuleFor(u => u.Username, f => f.Internet.UserName());


        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Username = faker.Username,
            });

        Assert.Equal(ogId, user!.Id);
        Assert.Equal(faker.FirstName, user!.FirstName);
        Assert.Equal(faker.LastName, user!.LastName);
        Assert.Equal(faker.Username, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesCorrectUser()
    {
        await using var db = CreateDbContext();

        var SeededUsers = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = SeededUsers[0].User;
        var user2 = SeededUsers[1].User;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        string newUsername = "NewCrazyUser-NameThatTotallyWontBeInBogusDB123xD!!()ADN";

        var service = new UserService(db);

        user1 = await service.UpdateCurrentUserAsync(
            user1.Id,
            new UpdateUserDTO
            {
                Username = newUsername
            });

        Assert.NotEqual(user1!.Id, user2!.Id);
        Assert.NotEqual(user1!.Username, user2!.Username);

    }

    [Fact]
    public async Task DeleteCurrentUserAsync_AdminDeletesUser()
    {
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var admin = seededUser[0].User;
        var user = seededUser[1].User;

        var organization = await OrganizationSeeder.SeedAsync(db, Seed);

        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: admin.Id, role: RoleType.Admin);
        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: user.Id, role: RoleType.Employee);

        var service = new UserService(db);

        Assert.Equal(UserServiceResult.Succes, await service.DeleteCurrentUserAsync(admin.Id, user.Id));

        var deletedUser = await db.Users.SingleOrDefaultAsync(u => u.Id == user.Id);
        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteCurrentUserAsync_AdminDeletesCorrectUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await UserSeeder.SeedManyAsync(db, Seed, 3);
        var admin = seededUsers[0].User;
        var user1 = seededUsers[1].User;
        var user2 = seededUsers[2].User;

        var organization = await OrganizationSeeder.SeedAsync(db, Seed);

        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: admin.Id, role: RoleType.Admin);
        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: user1.Id, role: RoleType.Employee);
        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: user2.Id, role: RoleType.Employee);

        var service = new UserService(db);

        Assert.Equal(UserServiceResult.Succes, await service.DeleteCurrentUserAsync(admin.Id, user1.Id));

        var deletedUser = await db.Users.SingleOrDefaultAsync(u => u.Id == user1.Id);
        Assert.Null(deletedUser);

        var remainingUser = await db.Users.SingleOrDefaultAsync(u => u.Id == user2.Id);
        Assert.NotNull(remainingUser);
    }

    [Fact]
    public async Task DeleteCurrentUserAsync_CannotDeleteNonExistingUser()
    {
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedAsync(db, Seed);
        var admin = seededUser.User;

        var organization = await OrganizationSeeder.SeedAsync(db, Seed);

        await UserRoleSeeder.SeedAsync(db, Seed, organizationId: organization.Id, nonCitizenId: admin.Id, role: RoleType.Admin);

        int nonExistingId = admin.Id + 100;

        var service = new UserService(db);

        // A non-existing user belongs to no organization, so the admin check fails before the lookup
        Assert.Equal(UserServiceResult.Forbidden, await service.DeleteCurrentUserAsync(admin.Id, nonExistingId));
    }

    [Fact]
    public async Task GetCurrentUserAsync_CannotGetNonExistingUser()
    {
        await using var db = CreateDbContext();

        int id = 1;

        var service = new UserService(db);

        var user = await service.GetCurrentUserAsync(id);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetCurrentUserAsync_GetsCurrentUser()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = SeededUser.User;

        var service = new UserService(db);

        var fetchedUser = await service.GetCurrentUserAsync(user.Id);

        Assert.Equal(fetchedUser!.Id, user.Id);
        Assert.Equal(fetchedUser!.FirstName, user.FirstName);
        Assert.Equal(fetchedUser!.LastName, user.LastName);
        Assert.Equal(fetchedUser!.UserName, user.Username);

    }

    [Fact]
    public async Task GetCurrentUserAsync_GetsCorrectUser()
    {
        await using var db = CreateDbContext();

        var SeededUsers = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = SeededUsers[0].User;
        var user2 = SeededUsers[1].User;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        var service = new UserService(db);

        var fetchedUser = await service.GetCurrentUserAsync(user2.Id);

        Assert.NotNull(fetchedUser);
        Assert.Equal(user2.Id, fetchedUser.Id);
        Assert.NotEqual(user1.Id, fetchedUser.Id);
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_DoesNothingOnNonExistingId()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = SeededUser.User;
        var password = SeededUser.PlainPassword;

        Assert.NotNull(user);

        // create a non-exsting id
        int nonId = user.Id + 2;
        string newPassword = "HestePeter2*2=4";

        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            nonId,
            new UpdatePasswordDTO
            {
                OldPassword = password,
                NewPassword = newPassword
            });

        Assert.Equal(0, result);

        var afterCallUser = await db.Users.SingleAsync(u => u.Id == user.Id);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, afterCallUser.Password));
        Assert.False(BCrypt.Net.BCrypt.Verify(newPassword, afterCallUser.Password));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_DoesNothingOnWrongPassword()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = SeededUser.User;
        var password = SeededUser.PlainPassword;

        Assert.NotNull(user);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user.Id,
            new UpdatePasswordDTO
            {
                OldPassword = "CorrectPasswordByHestePeter",
                NewPassword = newPassword
            });

        Assert.Equal(1, result);

        var afterCallUser = await db.Users.SingleAsync(u => u.Id == user.Id);
        Assert.True(BCrypt.Net.BCrypt.Verify(password, afterCallUser.Password));
        Assert.False(BCrypt.Net.BCrypt.Verify(newPassword, afterCallUser.Password));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_UpdatesPassword()
    {
        await using var db = CreateDbContext();

        var SeededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = SeededUser.User;
        var password = SeededUser.PlainPassword;

        Assert.NotNull(user);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user.Id,
            new UpdatePasswordDTO
            {
                OldPassword = password,
                NewPassword = newPassword
            });

        Assert.Equal(2, result);

        user = await db.Users.FindAsync(user.Id);
        Assert.NotNull(user);

        Assert.True(BCrypt.Net.BCrypt.Verify(newPassword, user!.Password));
    }

    [Fact]
    public async Task UpdateUserPasswordAsync_UpdatesCorrectUser()
    {
        await using var db = CreateDbContext();

        var SeededUsers = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = SeededUsers[0].User;
        var user2 = SeededUsers[1].User;
        var password = SeededUsers[0].PlainPassword;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user1.Id,
            new UpdatePasswordDTO
            {
                OldPassword = password,
                NewPassword = newPassword
            });

        Assert.Equal(2, result);


        Assert.False(BCrypt.Net.BCrypt.Verify(newPassword, user2!.Password));
    }

    // Virker ikke
    [Fact]
    public async Task Login_ReturnsExpectedDTOWithCorrectCredentials()
    {
        await using var db = CreateDbContext();

        var userInformation = await UserSeeder.SeedAsync(db, Seed);

        Assert.NotNull(userInformation.User);
        Assert.NotNull(userInformation.PlainPassword);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.Login(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username,
                Password = userInformation.PlainPassword
            });

        Assert.NotNull(result);
        Assert.Equal(userInformation.User.FirstName, result.FirstName);
        Assert.Equal(userInformation.User.LastName, result.LastName);
        Assert.Equal(userInformation.User.Username, result.Username);
        Assert.NotEmpty(result.Jwt);
    }

    // Virker ikke
    [Fact]
    public async Task Login_ReturnsNullWithIncorrectPassword()
    {
        await using var db = CreateDbContext();

        var userInformation = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.Login(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username,
                Password = userInformation.PlainPassword + "incorrect-password"
            });

        Assert.Null(result);
    }

    // Virker ikke
    [Fact]
    public async Task Login_ReturnsNullWithIncorrectUsername()
    {
        await using var db = CreateDbContext();

        var userInformation = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.Login(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username + "wrong-name",
                Password = userInformation.PlainPassword
            });

        Assert.Null(result);
    }

    // Virker ikke
    [Fact]
    public async Task Login_CorrectCredentialsReturnsValidJwt()
    {
        await using var db = CreateDbContext();

        var userInformation = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.Login(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username,
                Password = userInformation.PlainPassword
            });

        // Verify result contains a JWT.
        Assert.NotNull(result);
        Assert.NotEmpty(result.Jwt);

        // Validate JWT using environment variables.
        var tokenHandler = new JsonWebTokenHandler();

        var validationResult = await tokenHandler.ValidateTokenAsync(
            result.Jwt,
            TokenParameters.Get());

        Assert.True(
            validationResult.IsValid,
            validationResult.Exception?.ToString());
    }

    [Fact]
    public async Task UpdateUserEmailAsync_UpdatesEmail()
    {
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = (NonCitizen)seededUser.User;

        string newEmail = "new@test.dk";

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            user.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = user.Email,
                NewEmail = newEmail,
                Password = seededUser.PlainPassword
            });

        Assert.Equal(UserServiceResult.Succes, result);

        var updatedUser = await db.Users.OfType<NonCitizen>().SingleAsync(u => u.Id == user.Id);
        Assert.Equal(newEmail, updatedUser.Email);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_CitizenCannotHaveEmail()
    {
        await using var db = CreateDbContext();

        var citizen = await CitizenSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            citizen.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = "old@test.dk",
                NewEmail = "new@test.dk",
                Password = "HestePeter2*2=4"
            });

        Assert.Equal(UserServiceResult.EmailNotAllowedForCitizen, result);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_ReturnsNotFoundOnNonExistingUser()
    {
        await using var db = CreateDbContext();

        int id = 100;

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            id,
            new UpdateUserEmailDTO
            {
                OldEmail = "old@test.dk",
                NewEmail = "new@test.dk",
                Password = "HestePeter2*2=4"
            });

        Assert.Equal(UserServiceResult.NotFound, result);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_DoesNothingOnWrongPassword()
    {
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = (NonCitizen)seededUser.User;
        string ogEmail = user.Email;

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            user.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = ogEmail,
                NewEmail = "new@test.dk",
                Password = seededUser.PlainPassword + "incorrect-password"
            });

        Assert.Equal(UserServiceResult.WrongPassword, result);

        var afterCallUser = await db.Users.OfType<NonCitizen>().SingleAsync(u => u.Id == user.Id);
        Assert.Equal(ogEmail, afterCallUser.Email);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_DoesNothingOnWrongOldEmail()
    {
        await using var db = CreateDbContext();

        var seededUser = await UserSeeder.SeedAsync(db, Seed);
        var user = (NonCitizen)seededUser.User;
        string ogEmail = user.Email;

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            user.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = "wrong" + ogEmail,
                NewEmail = "new@test.dk",
                Password = seededUser.PlainPassword
            });

        Assert.Equal(UserServiceResult.WrongOldEmail, result);

        var afterCallUser = await db.Users.OfType<NonCitizen>().SingleAsync(u => u.Id == user.Id);
        Assert.Equal(ogEmail, afterCallUser.Email);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_DoesNothingWhenEmailIsTaken()
    {
        await using var db = CreateDbContext();

        var seededUsers = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = (NonCitizen)seededUsers[0].User;
        var user2 = (NonCitizen)seededUsers[1].User;
        string ogEmail = user1.Email;

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            user1.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = ogEmail,
                NewEmail = user2.Email, // already taken by user2
                Password = seededUsers[0].PlainPassword
            });

        Assert.Equal(UserServiceResult.EmailTaken, result);

        var afterCallUser = await db.Users.OfType<NonCitizen>().SingleAsync(u => u.Id == user1.Id);
        Assert.Equal(ogEmail, afterCallUser.Email);
    }

    [Fact]
    public async Task UpdateUserEmailAsync_UpdatesCorrectUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = (NonCitizen)seededUsers[0].User;
        var user2 = (NonCitizen)seededUsers[1].User;
        string ogEmailUser2 = user2.Email;

        var service = new UserService(db);

        var result = await service.UpdateUserEmailAsync(
            user1.Id,
            new UpdateUserEmailDTO
            {
                OldEmail = user1.Email,
                NewEmail = "new@test.dk",
                Password = seededUsers[0].PlainPassword
            });

        Assert.Equal(UserServiceResult.Succes, result);

        var otherUser = await db.Users.OfType<NonCitizen>().SingleAsync(u => u.Id == user2.Id);
        Assert.Equal(ogEmailUser2, otherUser.Email);
    }
}