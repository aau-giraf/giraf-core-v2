public static class UserRoleSeeder
{

    // Create a random UserRole and add it to mock database.
    public static async Task<UserRole> SeedAsync(AppDbContext db, int seed, int? organizationId = null, int? nonCitizenId = null, RoleType? role = null)
    {
        var userRole = Create(seed, organizationId, nonCitizenId, role);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public static async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, List<(int, int)> nonCitizenOrganizationTuples)
    {
        var userRoles = CreateManyFakers(seed, nonCitizenOrganizationTuples);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Generate a random userRole.
    private static UserRole Create(int seed, int? organizationId = null, int? nonCitizenId = null, RoleType? role = null)
    {
        return CreateFaker(seed, organizationId, nonCitizenId, role).Generate();
    }

    // Create x random userRoles.
    private static List<UserRole> CreateManyFakers(int seed, List<(int, int)> nonCitizenOrganizationTuples)
    {

        List<UserRole> userRoleFakers = [];

        foreach (var nonCitizenOrganizationTuple in nonCitizenOrganizationTuples)
        {
            userRoleFakers.Add(CreateFaker(seed, nonCitizenOrganizationTuple.Item1, nonCitizenOrganizationTuple.Item2).Generate());
        }
        return userRoleFakers;
    }

    // Create a random test userRole using bogus.
    private static Faker<UserRole> CreateFaker(int seed, int? organizationId = null, int? nonCitizenId = null, RoleType? role = null)
    {

        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => nonCitizenId != null ? nonCitizenId : null)
            .RuleFor(ur => ur.OrganizationId, f => organizationId != null ? organizationId : f.Random.Int(1, 1000))
            .RuleFor(ur => ur.Role, f => role != null ? role : f.PickRandom<RoleType>());
    }
}