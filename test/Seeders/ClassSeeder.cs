
public static class ClassSeeder
{

    // Create a random class and add it to mock database.
    public static async Task<Class> SeedAsync(AppDbContext db, int seed, Organization organization)
    {
        var class_ = Create(seed, organization);

        db.Classes.Add(class_);
        await db.SaveChangesAsync();

        return class_;
    }

    // Create x random classes and add them to mock database.
    public static async Task<List<Class>> SeedManyAsync(AppDbContext db, int seed, int count, Organization organization)
    {
        var classes = CreateManyFakers(seed, count, organization);

        db.Classes.AddRange(classes);
        await db.SaveChangesAsync();

        return classes;
    }

    // Generate a random class.
    private static Class Create(int seed, Organization organization)
    {
        return CreateFaker(seed, organization).Generate();
    }

    // Create x random classes.
    private static List<Class> CreateManyFakers(int seed, int count, Organization organization)
    {
        return CreateFaker(seed, organization).Generate(count);
    }

    // Create a random test class using bogus.
    private static Faker<Class> CreateFaker(int seed, Organization organization)
    {
        return new Faker<Class>()
            .UseSeed(seed)
            .RuleFor(c => c.Name, f => f.Name.FirstName())
            .RuleFor(c => c.OrganizationId, f => organization.Id);
    }

}