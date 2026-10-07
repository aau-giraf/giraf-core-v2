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
        string ogPassword = user.Password;

        var service = new UserService(db);

        user = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserDTO
            {
                FirstName = "New"
            });

        Assert.Equal(ogId, user!.Id);
        Assert.Equal("New", user!.FirstName);
        Assert.NotEqual(ogFirstname, user!.FirstName);
        Assert.Equal(ogLastname, user!.LastName);
        Assert.Equal(ogEmail, user!.Email);
        Assert.Equal(ogUsername, user!.Username);
        Assert.Equal(ogPassword, user!.Password);

    }
}
