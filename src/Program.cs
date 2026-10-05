using Microsoft.EntityFrameworkCore;
using giraf_core_v2.Data.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Register health checks
builder.Services.AddHealthChecks();

builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("defaultConnection"))
        .UseSeeding((context, _) => {
            var dbContext = (AppDbContext)context;
            OrganizationConfiguration.Seed(dbContext);
            UserConfiguration.Seed(dbContext);
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
