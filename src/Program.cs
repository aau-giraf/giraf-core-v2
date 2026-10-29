using System.Text.Json.Serialization;

using giraf_core_v2.Data.Seeding;

using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<CitizenService>();

builder.Services.AddValidation();

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

app.MapHealthChecks("/health");
app.MapOrganizationEndpoints();
app.MapUserEndpoints();
app.MapCitizenEndpoints();

app.Run();