
public static class ClassSeeder
{

    // Create a random class and add it to mock database.
    public static async Task<Class> SeedAsync(AppDbContext db, int seed, int? organizationId = 0)
    {
        var class_ = Create(seed, organizationId);

        db.Classes.Add(class_);
        await db.SaveChangesAsync();

        return class_;
    }

    // Create x random classes and add them to mock database.
    public static async Task<List<Class>> SeedManyAsync(AppDbContext db, int seed, int count, int? organizationId = 0)
    {
        var classes = CreateManyFakers(seed, count, organizationId);

        db.Classes.AddRange(classes);
        await db.SaveChangesAsync();

        return classes;
    }

    // Generate a random class.
    private static Class Create(int seed, int? organizationId)
    {
        return CreateFaker(seed, organizationId).Generate();
    }

    // Create x random classes.
    private static List<Class> CreateManyFakers(int seed, int count, int? organizationId)
    {
        return CreateFaker(seed, organizationId).Generate(count);
    }

    // Create a random test class using bogus.
    private static Faker<Class> CreateFaker(int seed, int? organizationId)
    {
        return new Faker<Class>()
            .UseSeed(seed)
            .RuleFor(c => c.Name, f => f.Name.FirstName())
            .RuleFor(c => c.OrganizationId, f => organizationId != 0 ? organizationId : f.Random.Int(1, 100));
    }

}