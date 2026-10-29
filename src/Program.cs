using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

using giraf_core_v2.Auth;
using giraf_core_v2.Data.Seeding;
using giraf_core_v2.Utils;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection"))
        .UseSeeding((context, _) =>
        { // Runs when Databse.Migrate.
            var seeders = typeof(AppDbContext).Assembly // Gets all classes from the assembly AppDbContext gets compiled into (all classes)
                .GetTypes()
                .Where(type => typeof(ISeeding).IsAssignableFrom(type) && !type.IsInterface) // Ensures only entities compatible with ISeeding are included (excluding ISeeding itself)
                .Select(type => (ISeeding)Activator.CreateInstance(type)!) // Creates instances of said classes
                .OrderBy(seeder => seeder.SeedingPosition); // Ensures seeding happens in the correct order in respect to FK linking

            foreach (ISeeding seeder in seeders)
            {
                seeder.Seed((AppDbContext)context);
            }
        })
);

builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(
        new JsonStringEnumConverter()));

builder.Services.AddScoped<OrganizationService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TokenProvider>();

builder.Services.AddValidation();


builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
{
    o.RequireHttpsMetadata = false;
    o.TokenValidationParameters = new TokenValidationParameters
    {
        // Validate signing key.
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT_SECRET"]!)),

        // Issuer and audience must match the ones specified in the environment variables.
        ValidateIssuer = true,
        ValidIssuer = Env.Get("JWT_ISSUER"),

        ValidateAudience = true,
        ValidAudience = Env.Get("JWT_AUDIENCES"),

        // The JWT must not be expired.
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGenWithAuth();

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

app.UseAuthentication();

app.UseAuthorization();

app.Run();