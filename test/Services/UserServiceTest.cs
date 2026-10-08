using Xunit.Abstractions;

public class UserServiceTest : TestBase
{
    public UserServiceTest(ITestOutputHelper output) : base(output) { }

    // TODO: add sanitization tests, once sanitization has been made.


    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesTheUser()
    {
        await using var db = CreateDbContext();

        var user = await UserSeeder.SeedAsync(db, Seed);

        // save origianl data, because user cannot be a constant entity.
        int ogId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogEmail = user.Email;
        string ogUsername = user.Username;

        // generate 'fake' user firstname using Bogus. (use different seed than us)
        User faker = new Faker<User>().UseSeed(Seed + 123)
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
        Assert.Equal(ogEmail, user!.Email);
        Assert.Equal(ogUsername, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesEntireUser()
    {
        await using var db = CreateDbContext();

        var user = await UserSeeder.SeedAsync(db, Seed);

        // save origianl data, because user cannot be a constant entity.
        int ogId = user.Id;
        string ogFirstname = user.FirstName;
        string ogLastname = user.LastName;
        string ogEmail = user.Email;
        string ogUsername = user.Username;

        // generate 'fake' user firstname using Bogus. (use different seed than us)
        User faker = new Faker<User>().UseSeed(Seed + 123)
                                    .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                                    .RuleFor(u => u.LastName, f => f.Name.LastName())
                                    .RuleFor(u => u.Email, f => f.Internet.Email())
                                    .RuleFor(u => u.Username, f => f.Internet.UserName());


        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserDTO
            {
                FirstName = faker.FirstName,
                LastName = faker.LastName,
                Email = faker.Email,
                Username = faker.Username,
            });

        Assert.Equal(ogId, user!.Id);
        Assert.Equal(faker.FirstName, user!.FirstName);
        Assert.Equal(faker.LastName, user!.LastName);
        Assert.Equal(faker.Email, user!.Email);
        Assert.Equal(faker.Username, user!.Username);

    }

    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesCorrectUser()
    {
        await using var db = CreateDbContext();

        var users = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = users[0];
        var user2 = users[1];

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
    public async Task DeleteCurrentUserAsync_DeletesCurrentUser()
    {
        await using var db = CreateDbContext();

        var user = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        Assert.True(await service.DeleteCurrentUserAsync(user.Id));

        var deletedUser = await db.Users
            .SingleOrDefaultAsync(u => u.Id == user.Id);

        Assert.Null(deletedUser);
    }

    [Fact]
    public async Task DeleteCurrentUserAsync_DeletesCorrectUser()
    {
        await using var db = CreateDbContext();

        var users = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = users[0];
        var user2 = users[1];

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
    public async Task DeleteCurrentUserAsync_CannotDeleteNonExistingUser()
    {
        await using var db = CreateDbContext();

        int id = 1;

        var service = new UserService(db);

        Assert.False(await service.DeleteCurrentUserAsync(id));
    }

    [Fact]
    public async Task GetCurrentUserAsync_CannotDeleteNonExistingUser()
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

        var user = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        var fetchedUser = await service.GetCurrentUserAsync(user.Id);

        Assert.Equal(fetchedUser!.Id, user.Id);
        Assert.Equal(fetchedUser!.FirstName, user.FirstName);
        Assert.Equal(fetchedUser!.LastName, user.LastName);
        Assert.Equal(fetchedUser!.Email, user.Email);
        Assert.Equal(fetchedUser!.Username, user.Username);

    }

    [Fact]
    public async Task GetCurrentUserAsync_GetsCorrectUser()
    {
        await using var db = CreateDbContext();

        var users = await UserSeeder.SeedManyAsync(db, Seed, 2);
        var user1 = users[0];
        var user2 = users[1];

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

        var user = await UserSeeder.SeedAsync(db, Seed);

        Assert.NotNull(user);

        // create a non-exsting id
        int nonId = user.Id + 2;
        string newPassword = "HestePeter2*2=4";

        var service = new UserService(db);

        int result = await service.UpdateUserPasswordAsync(
            nonId,
            new UpdatePasswordDTO
            {
                OldPassword = user.Password,
                NewPassword = newPassword
            });

        Assert.Equal(0, result);

        user = await service.GetCurrentUserAsync(user.Id);

        Assert.NotEqual(user!.Password, newPassword);
    }

    // TODO: Do rest of password tests when user registration has been made.
    //     [Fact]
    //     public async Task UpdateUserPasswordAsync_DoesNothingOnWrongPassword()
    //     {
    //         await using var db = CreateDbContext();
    //
    //         var user = await UserSeeder.SeedAsync(db, Seed);
    //
    //         Assert.NotNull(user);
    //
    //         string newPassword = "HestePeter2*2=4";
    //         var service = new UserService(db);
    //
    //         int result = await service.UpdateUserPasswordAsync(
    //             user.Id,
    //             new UpdatePasswordDTO
    //             {
    //                 OldPassword = "CorrectPasswordByHestePeter",
    //                 NewPassword = newPassword
    //             });
    //
    //         Assert.Equal(1, result);
    //
    //         user = await service.GetCurrentUserAsync(user.Id);
    //
    //         Assert.NotEqual(user!.Password, newPassword);
    //     }

}
