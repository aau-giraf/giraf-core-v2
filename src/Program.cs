using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Register health checks.
builder.Services.AddHealthChecks();

// Configure database connection.
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection")));

// Add services to the app.
builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<UserService>();

// Add service for DTO validation.
builder.Services.AddValidation();

// Make the API interactable through Swagger.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();

// Add endpoints.
app.MapHealthChecks("/health");
app.MapColorEndpoints();
app.MapUserEndpoints();

app.Run();