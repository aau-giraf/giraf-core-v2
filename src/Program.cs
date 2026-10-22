using System.Threading.RateLimiting;

using giraf_core_v2.Auth;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection")));

builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddValidation();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("rateLimitByIp", httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.Connection.RemoteIpAddress?.ToString(),
            factory: f => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromSeconds(10)
            }
    ));
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// Load environment variables
DotNetEnv.Env.Load();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

app.MapHealthChecks("/health");
app.MapOrganizationEndpoints();
app.MapUserEndpoints();

app.UseRateLimiter();

app.Run();