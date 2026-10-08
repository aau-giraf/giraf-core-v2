
public static class OrganizationSeeder
{
    
    // Create a random organization and add it to mock database.
    public static async Task<Organization> SeedAsync(AppDbContext db, int seed)
    {
        var organization = Create(seed);

        db.Organizations.Add(organization);
        await db.SaveChangesAsync();

        return organization;
    }

    // Create x random organizations and add them to mock database.
    public static async Task<List<Organization>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var organizations = CreateMany(seed, count);

        db.Organizations.AddRange(organizations);
        await db.SaveChangesAsync();

        return organizations;
    }

    // Generate a random organization.
    private static Organization Create(int seed)
    {
        return CreateFaker(seed).Generate();
    }

     // Create x random users.
    private static List<Organization> CreateMany(int seed, int count)
    {
        return CreateFaker(seed).Generate(count);
    }

     // Create a random test organization using bogus.
    private static Faker<Organization> CreateFaker(int seed)
    {
        return new Faker<Organization>()
            .UseSeed(seed)
            .RuleFor(o => o.Id, f => f.Random.Long())
            .RuleFor(o => o.Name, f => f.Name.FirstName());
    }
}