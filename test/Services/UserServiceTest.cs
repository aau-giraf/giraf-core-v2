using Microsoft.IdentityModel.JsonWebTokens;

namespace giraf_core_v2.Tests.Services;

public class UserServiceTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly UserSeeder _userSeeder = new();

    // TODO: add sanitization tests, once sanitization has been made.
    [Fact]
    public async Task UpdateCurrentUser_UserExists_UpdatesUser()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);

        var user = seededUser.User;

        // save origianl data, because user cannot be a constant entity.
        int originalUserId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogUsername = user.Username;

        // generate a different user with another name.
        var otherUser = _userSeeder.Create(Seed);


        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserRequestDTO
            {
                FirstName = otherUser.User.FirstName
            });

        Assert.Equal(originalUserId, user!.Id);
        Assert.Equal(otherUser.User.FirstName, user!.FirstName);
        Assert.Equal(ogLastname, user!.LastName);
        Assert.Equal(ogUsername, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUser_UserDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();

        int id = 1;


        var service = new UserService(db);

        var user = await service.UpdateCurrentUserAsync(
            id,
            new UpdateUserRequestDTO
            {
                FirstName = ":)"
            });

        Assert.Null(user);

    }

    [Fact]
    public async Task UpdateCurrentUser_AllFieldsProvided_UpdatesAllFields()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);

        var user = seededUser.User;

        // save origianl data, because user cannot be a constant entity.
        int originalUserId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogUsername = user.Username;

        // generate different user with another seed
        User faker = new Faker<NonCitizen>().UseSeed(Seed)
                                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                                    .RuleFor(u => u.Username, f => f.Internet.UserName());


        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserRequestDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Username = faker.Username,
            });

        Assert.Equal(originalUserId, user!.Id);
        Assert.Equal(faker.FirstName, user!.FirstName);
        Assert.Equal(faker.LastName, user!.LastName);
        Assert.Equal(faker.Username, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUser_MultipleUsersExist_UpdatesRequestedUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await _userSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = seededUsers[0].User;
        var user2 = seededUsers[1].User;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        string newUsername = "NewCrazyUser-NameThatTotallyWontBeInBogusDB123xD!!()ADN";

        var service = new UserService(db);

        user1 = await service.UpdateCurrentUserAsync(
            user1.Id,
            new UpdateUserRequestDTO
            {
                Username = newUsername
            });

        Assert.NotEqual(user1!.Id, user2!.Id);
        Assert.NotEqual(user1!.Username, user2!.Username);

    }

    [Fact]
    public async Task DeleteCurrentUser_UserExists_DeletesUser()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);
        var user = seededUser.User;

        var service = new UserService(db);

        Assert.True(await service.DeleteCurrentUserAsync(user.Id));

        var deletedUser = await db.Users
            .SingleOrDefaultAsync(u => u.Id == user.Id);

        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteCurrentUser_MultipleUsersExist_DeletesRequestedUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await _userSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = seededUsers[0].User;
        var user2 = seededUsers[1].User;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        var service = new UserService(db);

        Assert.True(await service.DeleteCurrentUserAsync(user1.Id));

        var deletedUser = await db.Users
            .SingleOrDefaultAsync(u => u.Id == user1.Id);

        Assert.Null(deletedUser);

        // Make sure the other user was not accidentally deleted
        var remainingUser = await db.Users
            .SingleOrDefaultAsync(u => u.Id == user2.Id);

        Assert.NotNull(remainingUser);
    }

    [Fact]
    public async Task DeleteCurrentUser_UserDoesNotExist_ReturnsFalse()
    {
        await using var db = CreateDbContext();

        int id = 1;

        var service = new UserService(db);

        Assert.False(await service.DeleteCurrentUserAsync(id));
    }

    [Fact]
    public async Task GetCurrentUser_UserDoesNotExist_ReturnsNull()
    {
        await using var db = CreateDbContext();

        int id = 1;

        var service = new UserService(db);

        var user = await service.GetCurrentUserAsync(id);

        Assert.Null(user);
    }

    [Fact]
    public async Task GetCurrentUser_UserExists_ReturnsUser()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);
        var user = seededUser.User;

        var service = new UserService(db);

        var fetchedUser = await service.GetCurrentUserAsync(user.Id);

        Assert.Equal(fetchedUser!.Id, user.Id);
        Assert.Equal(fetchedUser!.FirstName, user.FirstName);
        Assert.Equal(fetchedUser!.LastName, user.LastName);
        Assert.Equal(fetchedUser!.Username, user.Username);

    }

    [Fact]
    public async Task GetCurrentUser_MultipleUsersExist_ReturnsRequestedUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await _userSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = seededUsers[0].User;
        var user2 = seededUsers[1].User;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        var service = new UserService(db);

        var fetchedUser = await service.GetCurrentUserAsync(user2.Id);

        Assert.NotNull(fetchedUser);
        Assert.Equal(user2.Id, fetchedUser.Id);
        Assert.NotEqual(user1.Id, fetchedUser.Id);
    }

    [Fact]
    public async Task UpdateUserPassword_UserDoesNotExist_ReturnsNotFound()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);
        var user = seededUser.User;
        var password = seededUser.PlainPassword;

        Assert.NotNull(user);

        // create a non-exsting id
        int missingUserId = user.Id + 2;
        string newPassword = "HestePeter2*2=4";

        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            missingUserId,
            new UpdatePasswordRequestDTO
            {
                OldPassword = password,
                NewPassword = newPassword
            });

        Assert.Equal(0, result);

        user = await service.GetCurrentUserAsync(user.Id);

        Assert.NotEqual(user!.Password, newPassword);
    }

    [Fact]
    public async Task UpdateUserPassword_PasswordIsWrong_ReturnsUnauthorized()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);
        var user = seededUser.User;

        Assert.NotNull(user);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user.Id,
            new UpdatePasswordRequestDTO
            {
                OldPassword = "CorrectPasswordByHestePeter",
                NewPassword = newPassword
            });

        Assert.Equal(1, result);

        user = await service.GetCurrentUserAsync(user.Id);

        Assert.NotEqual(user!.Password, newPassword);
    }

    [Fact]
    public async Task UpdateUserPassword_PasswordIsCorrect_UpdatesPassword()
    {
        await using var db = CreateDbContext();

        var seededUser = await _userSeeder.SeedAsync(db, Seed);
        var user = seededUser.User;
        var password = seededUser.PlainPassword;

        Assert.NotNull(user);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user.Id,
            new UpdatePasswordRequestDTO
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
    public async Task UpdateUserPassword_MultipleUsersExist_UpdatesRequestedUser()
    {
        await using var db = CreateDbContext();

        var seededUsers = await _userSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = seededUsers[0].User;
        var user2 = seededUsers[1].User;
        var password = seededUsers[0].PlainPassword;

        Assert.NotNull(user1);
        Assert.NotNull(user2);

        string newPassword = "HestePeter2*2=4";
        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            user1.Id,
            new UpdatePasswordRequestDTO
            {
                OldPassword = password,
                NewPassword = newPassword
            });

        Assert.Equal(2, result);


        Assert.False(BCrypt.Net.BCrypt.Verify(newPassword, user2!.Password));
    }

    [Fact]
    public async Task Login_CredentialsAreCorrect_ReturnsLoginResponse()
    {
        await using var db = CreateDbContext();

        var userInformation = await _userSeeder.SeedAsync(db, Seed);

        Assert.NotNull(userInformation.User);
        Assert.NotNull(userInformation.PlainPassword);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.LoginAsync(
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

    [Fact]
    public async Task Login_PasswordIsIncorrect_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var userInformation = await _userSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.LoginAsync(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username,
                Password = userInformation.PlainPassword + "incorrect-password"
            });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_UsernameIsIncorrect_ReturnsNull()
    {
        await using var db = CreateDbContext();

        var userInformation = await _userSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.LoginAsync(
            new LoginRequestDTO
            {
                Username = userInformation.User.Username + "wrong-name",
                Password = userInformation.PlainPassword
            });

        Assert.Null(result);
    }

    [Fact]
    public async Task Login_CredentialsAreCorrect_ReturnsValidJwt()
    {
        await using var db = CreateDbContext();

        var userInformation = await _userSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        LoginResponseDTO? result = await service.LoginAsync(
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
}