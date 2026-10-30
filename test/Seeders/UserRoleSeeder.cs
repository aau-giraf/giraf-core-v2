public static class UserRoleSeeder
{

    // Create a random UserRole and add it to mock database.
    public static async Task<UserRole> SeedAsync(AppDbContext db, int seed)
    {
        var userRole = Create(seed);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public static async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var userRoles = CreateManyFakers(seed, count);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Generate a random userRole.
    private static UserRole Create(int seed)
    {
        return CreateFaker(seed).Generate();
    }

    // Create x random userRoles.
    private static List<UserRole> CreateManyFakers(int seed, int count)
    {
        return CreateFaker(seed).Generate(count);
    }

    // Create a random test userRole using bogus.
    private static Faker<UserRole> CreateFaker(int seed)
    {
        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => f.Random.Int())
            .RuleFor(ur => ur.OrganizationId, f => f.Random.Int())
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());
    }
}