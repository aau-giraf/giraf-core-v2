using System.Text.Json.Serialization;

using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

// Starts the real Foodplanner legacy routes on a free port, backed by an in-memory database.
// Shared by every Foodplanner adapter test: use Client to call an endpoint and CreateDbContext to seed or check data.
// The service setup copies the parts of Program.cs the legacy endpoints depend on
public sealed class FoodplannerTestApp : IAsyncLifetime
{
    private readonly string _databaseName = Guid.NewGuid().ToString();
    private WebApplication? _app;

    public WebApplication App => _app ?? throw new InvalidOperationException("The app has not started yet");

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");

        builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase(_databaseName));
        builder.Services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
        builder.Services.AddScoped<OrganizationService>();
        builder.Services.AddScoped<UserService>();
        builder.Services.AddLegacyAdapters();
        builder.Services.AddValidation();

        _app = builder.Build();
        _app.MapFoodplannerLegacyApi();

        await _app.StartAsync();
        Client = new HttpClient { BaseAddress = new Uri(_app.Urls.First()) };
    }

    // A context on the same in-memory database the app uses. Dispose it when done
    public AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(_databaseName)
            .Options;

        return new AppDbContext(options);
    }

    // Sends a request as the given v2 user, using the temporary header from FoodplannerCallerResolver
    public HttpRequestMessage RequestAs(int? userId, HttpMethod method, string path, HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, path) { Content = content };
        if (userId is not null)
        {
            request.Headers.Add("X-Dev-User-Id", userId.ToString());
        }

        return request;
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}