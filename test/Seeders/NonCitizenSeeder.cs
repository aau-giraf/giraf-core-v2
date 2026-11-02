using Microsoft.AspNetCore.Authorization.Infrastructure;

using Xunit.Sdk;

public static class NonCitizenSeeder
{
    // Create a random nonCitizen and add it to mock database.
    public static async Task<NonCitizen> SeedAsync(AppDbContext db, int seed)
    {

        var nonCitizen = Create(seed);

        db.NonCitizens.Add(nonCitizen);
        await db.SaveChangesAsync();

        return nonCitizen;

    }

    // Create x random nonCitizens and add them to mock database.
    public static async Task<List<NonCitizen>> SeedManyAsync(AppDbContext db, int seed, int count)
    {

        var nonCitizens = CreateMany(seed, count);

        db.NonCitizens.AddRange(nonCitizens);
        await db.SaveChangesAsync();

        return nonCitizens;

    }

    // Generate a random nonCitizen.
    private static NonCitizen Create(int seed)
    {
        return CreateFaker(seed).Generate();
    }

    // Create x random nonCitizens.
    private static List<NonCitizen> CreateMany(int seed, int count)
    {
        return CreateFaker(seed).Generate(count);
    }

    // Create a random test nonCitizen using bogus.
    private static Faker<NonCitizen> CreateFaker(int seed)
    {
        var nonCitizenIdFaker = new Faker<NonCitizen>()
            .UseSeed(seed)
            .RuleFor(nc => nc.Id, f => f.Random.Int(1, 1000000));

        var nonCitizenId = nonCitizenIdFaker.Generate();

        var userRoleFaker = new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => nonCitizenId.Id)
            .RuleFor(ur => ur.OrganizationId, f => f.Random.Number(1, 100000))
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());

        var userRole = userRoleFaker.Generate();

        return new Faker<NonCitizen>()
            .UseSeed(seed)
            .CustomInstantiator(f => new NonCitizen(
                userRole.NonCitizenId,
                f.Name.FirstName(),
                f.Name.LastName(),
                f.Internet.UserName(),
                f.Internet.Email(),
                BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12),
                userRoleFaker.Generate(f.Random.Number(1, 100)).ToList()
                )
            );
        /*
        .RuleFor(nc => nc.Id, f => userRole.NonCitizenId)
        .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
        .RuleFor(nc => nc.LastName, f => f.Name.LastName())
        .RuleFor(nc => nc.Username, f => f.Internet.UserName())
        .RuleFor(nc => nc.Email, f => f.Internet.Email())
        .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12))
        .RuleFor(nc => nc.Roles, f => userRoleFaker.Generate(f.Random.Number(1, 100)).ToList());
        */
    }
}