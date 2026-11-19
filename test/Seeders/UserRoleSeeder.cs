// Record of optional parameters. Non-positive numbers result in random postive integer.
namespace giraf_core_v2.Tests.Seeders;

internal record UserRoleOptions(int OrganizationId = -9999, int NonCitizenId = -9999);

internal class UserRoleSeeder : Seeder<UserRole, UserRoleOptions>
{

    // Create a random UserRole and add it to mock database.
    public override async Task<UserRole> SeedAsync(AppDbContext db, int seed, UserRoleOptions? options = null)
    {
        var userRole = Create(seed, options);

        db.UserRoles.Add(userRole);
        await db.SaveChangesAsync();

        return userRole;
    }

    // Create x random UserRole and add them to mock database.
    public override async Task<List<UserRole>> SeedManyAsync(AppDbContext db, int seed, int count, List<UserRoleOptions>? options = null)
    {
        var userRoles = CreateMany(seed, count, options);

        db.UserRoles.AddRange(userRoles);
        await db.SaveChangesAsync();

        return userRoles;
    }

    // Create a random test userRole using bogus.
    public override Faker<UserRole> CreateFaker(int seed, UserRoleOptions? options)
    {
        return new Faker<UserRole>()
            .UseSeed(seed)
            .RuleFor(ur => ur.NonCitizenId, f => options is not null && options.NonCitizenId > 0 ? options.NonCitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(ur => ur.OrganizationId, f => options is not null && options.OrganizationId > 0 ? options.OrganizationId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(ur => ur.Role, f => f.PickRandom<RoleType>());
    }
}