// Record of optional parameters. Non-positive numbers result in random postive integer.
namespace giraf_core_v2.Tests.Seeders;

internal record GuardianshipOptions(int CitizenId = -9999, int NonCitizenId = -9999);

internal class GuardianshipSeeder : Seeder<Guardianship, GuardianshipOptions>
{

    // Create a random Guardianship and add it to mock database.
    public override async Task<Guardianship> SeedAsync(AppDbContext db, int seed, GuardianshipOptions? options = null)
    {
        var guardianship = Create(seed, options);

        db.Guardianships.Add(guardianship);
        await db.SaveChangesAsync();

        return guardianship;
    }

    // Create x random Guardianship and add them to mock database.
    public override async Task<List<Guardianship>> SeedManyAsync(AppDbContext db, int seed, int count, List<GuardianshipOptions>? options = null)
    {
        var guardianships = CreateMany(seed, count, options);

        db.Guardianships.AddRange(guardianships);
        await db.SaveChangesAsync();

        return guardianships;
    }

    // Create a random test Guardianship using bogus.
    public override Faker<Guardianship> CreateFaker(int seed, GuardianshipOptions? options = null)
    {
        return new Faker<Guardianship>()
            .UseSeed(seed)
            .RuleFor(gs => gs.CitizenId, f => options is not null && options.CitizenId > 0 ? options.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(gs => gs.GuardianId, f => options is not null && options.NonCitizenId > 0 ? options.NonCitizenId : f.Random.Int(1, Int32.MaxValue));
    }
}