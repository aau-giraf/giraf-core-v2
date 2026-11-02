public static class UserRoleSeeder
{

    // Create a random UserRole and add it to mock database.
    public static async Task<UserRole> SeedAsync(AppDbContext db, int seed, int? nonCitizenId = 0)
    {
        var userRole = Create(seed, nonCitizenId);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public static async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, int count, int? nonCitizenId = 0)
    {
        var userRoles = CreateManyFakers(seed, count, nonCitizenId);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Generate a random userRole.
    private static UserRole Create(int seed, int? nonCitizenId = 0)
    {
        return CreateFaker(seed, nonCitizenId).Generate();
    }

    // Create x random userRoles.
    private static List<UserRole> CreateManyFakers(int seed, int count, int? nonCitizenId = 0)
    {
        return CreateFaker(seed, nonCitizenId).Generate(count);
    }

    // Create a random test userRole using bogus.
    private static Faker<UserRole> CreateFaker(int seed, int? nonCitizenId = 0)
    {

        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => nonCitizenId != 0 ? nonCitizenId : f.Random.Int(1, 1000000))
            .RuleFor(ur => ur.OrganizationId, f => f.Random.Int(1, 100000))
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());
    }
}