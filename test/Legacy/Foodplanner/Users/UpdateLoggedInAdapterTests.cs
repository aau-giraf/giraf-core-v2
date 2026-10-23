using System.Net;
using System.Text;

using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;
using giraf_core_v2.Legacy.Foodplanner.Users;

using Xunit.Abstractions;

// Two kinds of tests, which every Foodplanner adapter should have:
// - Adapter tests call HandleAsync directly with a real v2 service on an in-memory database. They cover the behaviour
// - HTTP tests call the real legacy URL through FoodplannerTestApp. They prove the route, the JSON body and the caller header
public class UpdateLoggedInAdapterTests : TestBase, IClassFixture<FoodplannerTestApp>
{
    private const string Path = "/legacy/foodplanner/api/Users/UpdateLoggedIn";

    private readonly FoodplannerTestApp _app;

    public UpdateLoggedInAdapterTests(ITestOutputHelper output, FoodplannerTestApp app) : base(output)
    {
        _app = app;
    }

    private static UpdateLoggedInRequest Request(string firstName, string lastName, string email)
    {
        return new UpdateLoggedInRequest(new UserUpdateLoggedInDTO { FirstName = firstName, LastName = lastName, Email = email });
    }

    private static LegacyCallContext CalledBy(int? userId)
    {
        return new LegacyCallContext(userId, "http://localhost/legacy/foodplanner");
    }

    private static StringContent Json(string json)
    {
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    // Adapter tests

    [Fact]
    public async Task HandleAsync_WithoutCaller_Returns401()
    {
        await using var db = CreateDbContext();
        var adapter = new UpdateLoggedInAdapter(new UserService(db));

        var result = await adapter.HandleAsync(Request("Anna", "Jensen", "anna@example.com"), CalledBy(null), CancellationToken.None);

        Assert.Equal(401, result.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_UnknownUser_Returns404()
    {
        await using var db = CreateDbContext();
        var adapter = new UpdateLoggedInAdapter(new UserService(db));

        var result = await adapter.HandleAsync(Request("Anna", "Jensen", "anna@example.com"), CalledBy(12345), CancellationToken.None);

        Assert.Equal(404, result.StatusCode);
    }

    [Fact]
    public async Task HandleAsync_UpdatesFirstAndLastName_Returns201()
    {
        await using var db = CreateDbContext();
        var seeded = await UserSeeder.SeedAsync(db, Seed);
        var adapter = new UpdateLoggedInAdapter(new UserService(db));

        var result = await adapter.HandleAsync(Request("Anna", "Jensen", "anna@example.com"), CalledBy(seeded.User.Id), CancellationToken.None);

        var user = await db.Users.FindAsync(seeded.User.Id);
        Assert.Equal(201, result.StatusCode);
        Assert.Equal("Anna", user!.FirstName);
        Assert.Equal("Jensen", user.LastName);
    }

    [Fact]
    public async Task HandleAsync_DoesNotChangeEmail()
    {
        await using var db = CreateDbContext();
        var seeded = await UserSeeder.SeedAsync(db, Seed);
        string originalEmail = ((NonCitizen)seeded.User).Email;
        var adapter = new UpdateLoggedInAdapter(new UserService(db));

        await adapter.HandleAsync(Request("Anna", "Jensen", "new@example.com"), CalledBy(seeded.User.Id), CancellationToken.None);

        var user = await db.NonCitizens.FindAsync(seeded.User.Id);
        Assert.Equal(originalEmail, user!.Email);
    }

    // HTTP tests

    [Fact]
    public async Task Put_UpdatesLoggedInUser_Returns201WithEmptyBody()
    {
        await using var db = _app.CreateDbContext();
        var seeded = await UserSeeder.SeedAsync(db, Seed);

        var response = await _app.Client.SendAsync(_app.RequestAs(seeded.User.Id, HttpMethod.Put, Path,
            Json("""{"firstName":"Anna","lastName":"Jensen","email":"anna@example.com"}""")));

        await using var check = _app.CreateDbContext();
        var user = await check.Users.FindAsync(seeded.User.Id);
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("", await response.Content.ReadAsStringAsync());
        Assert.Equal("Anna", user!.FirstName);
        Assert.Equal("Jensen", user.LastName);
    }

    [Fact]
    public async Task Put_WithoutCaller_Returns401()
    {
        var response = await _app.Client.SendAsync(_app.RequestAs(null, HttpMethod.Put, Path,
            Json("""{"firstName":"Anna","lastName":"Jensen","email":"anna@example.com"}""")));

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    // The old endpoint never checked its [Required]/[EmailAddress] attributes, so neither does the adapter
    [Fact]
    public async Task Put_InvalidEmail_IsNotValidatedLikeTheOldEndpoint()
    {
        await using var db = _app.CreateDbContext();
        var seeded = await UserSeeder.SeedAsync(db, Seed);

        var response = await _app.Client.SendAsync(_app.RequestAs(seeded.User.Id, HttpMethod.Put, Path,
            Json("""{"firstName":"Anna","lastName":"Jensen","email":"not-an-email"}""")));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    // The old backend crashed with 500 here. v2 rejects the body before the adapter runs
    [Fact]
    public async Task Put_MissingField_Returns400()
    {
        await using var db = _app.CreateDbContext();
        var seeded = await UserSeeder.SeedAsync(db, Seed);

        var response = await _app.Client.SendAsync(_app.RequestAs(seeded.User.Id, HttpMethod.Put, Path,
            Json("""{"firstName":"Anna"}""")));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}