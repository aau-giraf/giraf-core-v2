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
        return new Faker<NonCitizen>()
            .UseSeed(seed)
            .RuleFor(nc => nc.Id, f => f.Random.Int())
            .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
            .RuleFor(nc => nc.LastName, f => f.Name.LastName())
            .RuleFor(nc => nc.Username, f => f.Internet.UserName())
            .RuleFor(nc => nc.Email, f => f.Internet.Email())
            .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12));
    }
}