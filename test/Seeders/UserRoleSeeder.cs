// Record of optional parameters. Non-positive numbers result in random postive integer.
internal record UserRoleOptions(int OrgId = -9999, int NonCitizenId = -9999);

internal class UserRoleSeeder : Seeder<UserRole, UserRoleOptions>
{

    // Create a random UserRole and add it to mock database.
    public override async Task<UserRole> SeedAsync(AppDbContext db, int seed, UserRoleOptions? opt = null)
    {
        var userRole = Create(seed, opt);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public override async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, int count, List<UserRoleOptions>? opt = null)
    {
        var userRoles = CreateMany(seed, count, opt);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Create a random test userRole using bogus.
    public override Faker<UserRole> CreateFaker(int seed, UserRoleOptions? opt)
    {
        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => opt is not null && opt.NonCitizenId > 0 ? opt.NonCitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(ur => ur.OrganizationId, f => opt is not null && opt.OrgId > 0 ? opt.OrgId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());
    }
}