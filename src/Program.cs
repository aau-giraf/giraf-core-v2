using System.Text;
using System.Threading.RateLimiting;

using giraf_core_v2.Auth;
using giraf_core_v2.Utils;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection")));

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