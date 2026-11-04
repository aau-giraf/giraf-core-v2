using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

namespace giraf_core_v2.Tests;

// Starts the real API in memory, backed by a throwaway Postgres container.
public class GirafApiFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    // A fresh Postgres in Docker, created for the test run and deleted afterwards.
    [Obsolete]
    private readonly PostgreSqlContainer _db = new PostgreSqlBuilder()
        .WithImage("postgres:17")
        .Build();

    // Runs before Program.cs builds the app.
    // Replaces "defaultConnection" so the app talks to the container, never a real database.
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("ConnectionStrings:defaultConnection", _db.GetConnectionString());
    }

    // xUnit calls this before the first test in the class: start the container.
    public Task InitializeAsync() => _db.StartAsync();

    // xUnit calls this after the last test in the class: stop and delete the container.
    public new Task DisposeAsync() => _db.DisposeAsync().AsTask();

    // Gives the test a DbContext against the container, so it can insert its own data.
    public async Task<T> ArrangeAsync<T>(Func<AppDbContext, Task<T>> arrange)
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        return await arrange(db);
    }
}