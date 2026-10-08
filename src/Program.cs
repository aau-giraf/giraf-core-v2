using Microsoft.EntityFrameworkCore;
using giraf_core_v2.Data.Seeding;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection"))
        .UseSeeding((context, _) => { // Runs when Databse.Migrate. Contents a
            var seeders = typeof(AppDbContext).Assembly // Gets all classes from the assembly AppDbContext gets compiled into (all classes)
                .GetTypes()
                .Where(type => typeof(ISeeding).IsAssignableFrom(type) && !type.IsInterface) // Ensures only entities compatible with ISeeding are included (excluding ISeeding itself)
                .Select(type => (ISeeding)Activator.CreateInstance(type)!)
                .OrderBy(seeder => seeder.SeedingPosition); // Creates instances of said classes

            foreach (ISeeding seeder in seeders) {
                seeder.Seed((AppDbContext)context);
            }
        })
);


builder.Services.AddScoped<ColorService>();
builder.Services.AddScoped<UserService>();

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
app.MapColorEndpoints();
app.MapUserEndpoints();


app.Run();
