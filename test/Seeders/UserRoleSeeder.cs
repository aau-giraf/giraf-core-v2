public static class UserRoleSeeder
{

    // Create a random UserRole and add it to mock database.
    public static async Task<UserRole> SeedAsync(AppDbContext db, int seed, int? organizationId = null, int? nonCitizenId = null)
    {
        var userRole = Create(seed, organizationId, nonCitizenId);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public static async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, int count, int? organizationId = null, int? nonCitizenId = null)
    {
        var userRoles = CreateManyFakers(seed, count, organizationId, nonCitizenId);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Generate a random userRole.
    private static UserRole Create(int seed, int? organizationId = null, int? nonCitizenId = null)
    {
        return CreateFaker(seed, organizationId, nonCitizenId).Generate();
    }

    // Create x random userRoles.
    private static List<UserRole> CreateManyFakers(int seed, int count, int? organizationId = null, int? nonCitizenId = null)
    {
        return CreateFaker(seed, organizationId, nonCitizenId).Generate(count);
    }

    // Create a random test userRole using bogus.
    private static Faker<UserRole> CreateFaker(int seed, int? organizationId = null, int? nonCitizenId = null)
    {

        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => nonCitizenId != null ? nonCitizenId : null)
            .RuleFor(ur => ur.OrganizationId, f => organizationId != null ? organizationId : f.Random.Int(1, 100))
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());
    }
}