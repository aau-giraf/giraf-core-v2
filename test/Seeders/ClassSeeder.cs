
public static class ClassSeeder
{
    
    // Create a random class and add it to mock database.
    public static async Task<Class> SeedAsync(AppDbContext db, int seed)
    {
        var girafClass = Create(seed);

        db.Classes.Add(girafClass);
        await db.SaveChangesAsync();

        return girafClass;
    }

    // Create x random classes and add them to mock database.
    public static async Task<List<Class>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var classes = CreateMany(seed, count);

        db.Classes.AddRange(classes);
        await db.SaveChangesAsync();

        return classes;
    }

    // Generate a random class.
    private static Class Create(int seed)
    {
        return CreateFaker(seed).Generate();
    }

     // Create x random classes.
    private static List<Class> CreateMany(int seed, int count)
    {
        return CreateFaker(seed).Generate(count);
    }

     // Create a random test class using bogus.
    private static Faker<Class> CreateFaker(int seed)
    {
        return new Faker<Class>()
            .UseSeed(seed)
            .RuleFor(c => c.Id, f => f.Random.Long())
            .RuleFor(c => c.Name, f => f.Name.FirstName())
            .RuleFor(c => c.OrganizationId, f => f.Random.Long());
    }
}