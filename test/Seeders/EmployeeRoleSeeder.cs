// Record of optional parameters. Non-positive numbers result in random postive integer.
namespace giraf_core_v2.Tests.Seeders;

internal record EmployeeRoleOptions(int OrganizationId = -9999, int NonCitizenId = -9999);

internal class EmployeeRoleSeeder : Seeder<EmployeeRole, EmployeeRoleOptions>
{

    // Create a random EmployeeRole and add it to mock database.
    public override async Task<EmployeeRole> SeedAsync(AppDbContext db, int seed, EmployeeRoleOptions? options = null)
    {
        var employeeRole = Create(seed, options);

        db.EmployeeRoles.Add(employeeRole);
        await db.SaveChangesAsync();

        return employeeRole;
    }

    // Create x random EmployeeRole and add them to mock database.
    public override async Task<List<EmployeeRole>> SeedManyAsync(AppDbContext db, int seed, int count, List<EmployeeRoleOptions>? options = null)
    {
        var employeeRoles = CreateMany(seed, count, options);

        db.EmployeeRoles.AddRange(employeeRoles);
        await db.SaveChangesAsync();

        return employeeRoles;
    }

    // Create a random test employee role using bogus.
    public override Faker<EmployeeRole> CreateFaker(int seed, EmployeeRoleOptions? options)
    {
        return new Faker<EmployeeRole>()
            .UseSeed(seed)
            .RuleFor(employeeRole => employeeRole.NonCitizenId, f => options is not null && options.NonCitizenId > 0 ? options.NonCitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(employeeRole => employeeRole.OrganizationId, f => options is not null && options.OrganizationId > 0 ? options.OrganizationId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(employeeRole => employeeRole.Role, f => f.PickRandom<RoleType>());
    }
}