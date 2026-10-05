using Xunit.Abstractions;

public class UserServiceTest : TestBase
{
    public UserServiceTest(ITestOutputHelper output): base(output) {}

    [Fact]
    public async Task UpdateCurrentUserAsync_UpdatesTheUser()
    {
        await using var db = CreateDbContext();

        var user = await UserSeeder.SeedAsync(db, Seed);

        var service = new UserService(db);

        var updated = await service.UpdateCurrentUserAsync(
            user.Id,
            new UpdateUserDTO
            {
                FirstName = "New"
            });

        Assert.Equal("New", updated!.FirstName);
    }
}
