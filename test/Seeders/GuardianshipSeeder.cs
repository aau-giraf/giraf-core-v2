public static class GuardianshipSeeder
{

    // Create a random Guardianship and add it to mock database.
    public static async Task<Guardianship> SeedAsync(AppDbContext db, int seed, int? citizenId = null, int? nonCitizenId = null)
    {
        var guardianship = Create(seed, citizenId, nonCitizenId);

        db.Guardianships.Add(guardianship);
        await db.SaveChangesAsync();

        return guardianship;
    }

    // Create x random Guardianship and add them to mock database.
    public static async Task<List<Guardianship>> SeedManyAsync(AppDbContext db, int seed, List<(int, int)> citizenNonCitizenTuples)
    {
        var guardianships = CreateManyFakers(seed, citizenNonCitizenTuples);

        db.Guardianships.AddRange(guardianships);
        await db.SaveChangesAsync();

        return guardianships;
    }

    // Generate a random Guardianship.
    private static Guardianship Create(int seed, int? citizenId = null, int? nonCitizenId = null)
    {
        return CreateFaker(seed, citizenId, nonCitizenId).Generate();
    }

    // Create x random Guardianships
    private static List<Guardianship> CreateManyFakers(int seed, List<(int, int)> citizenNonCitizenTuples)
    {
        List<Guardianship> guardianFakers = [];

        foreach (var citizenNonCitizenTuple in citizenNonCitizenTuples)
        {
            guardianFakers.Add(CreateFaker(seed, citizenNonCitizenTuple.Item1, citizenNonCitizenTuple.Item2).Generate());
        }
        return guardianFakers;
    }

    // Create a random test Guardianship using bogus.
    private static Faker<Guardianship> CreateFaker(int seed, int? citizenId = null, int? nonCitizenId = null)
    {

        return new Faker<Guardianship>()
            .UseSeed(seed)
            .RuleFor(gs => gs.CitizenId, f => citizenId != null ? citizenId : f.Random.Int(1, 1000000))
            .RuleFor(gs => gs.GuardianId, f => nonCitizenId != null ? nonCitizenId : f.Random.Int(1, 1000000));
    }
}