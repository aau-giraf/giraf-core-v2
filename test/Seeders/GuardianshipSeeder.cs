// Record of optional parameters. Non-positive numbers result in random postive integer.
internal record GuardianshipOptions(int CitizenId = -9999, int NonCitizenId = -9999);

internal class GuardianshipSeeder : Seeder<Guardianship, GuardianshipOptions>
{

    // Create a random Guardianship and add it to mock database.
    public override async Task<Guardianship> SeedAsync(AppDbContext db, int seed, GuardianshipOptions? opt = null)
    {
        var guardianship = Create(seed, opt);

        db.Guardianships.Add(guardianship);
        await db.SaveChangesAsync();

        return guardianship;
    }

    // Create x random Guardianship and add them to mock database.
    public override async Task<List<Guardianship>> SeedManyAsync(AppDbContext db, int seed, int count, List<GuardianshipOptions?>? opt = null)
    {
        var guardianships = CreateMany(seed, count, opt);

        db.Guardianships.AddRange(guardianships);
        await db.SaveChangesAsync();

        return guardianships;
    }

    // Create a random test Guardianship using bogus.
    public override Faker<Guardianship> CreateFaker(int seed, GuardianshipOptions? opt = null)
    {
        return new Faker<Guardianship>()
            .UseSeed(seed)
            .RuleFor(gs => gs.CitizenId, f => opt is not null && opt.CitizenId > 0 ? opt.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(gs => gs.GuardianId, f => opt is not null && opt.NonCitizenId > 0 ? opt.NonCitizenId : f.Random.Int(1, Int32.MaxValue));
    }
}