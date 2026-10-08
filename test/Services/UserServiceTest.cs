using Xunit.Abstractions;

public class UserServiceTest : TestBase
{
    public UserServiceTest(ITestOutputHelper output): base(output) {}

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
}
