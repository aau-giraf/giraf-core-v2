// Options for creating org, if not supplied a random name is generated.
namespace giraf_core_v2.Tests.Seeders;

internal record OrganizationOptions(string OrganizationName = "");

internal class OrganizationSeeder : Seeder<Organization, OrganizationOptions>
{

    // Create a random organization and add it to mock database.
    public override async Task<Organization> SeedAsync(AppDbContext db, int seed, OrganizationOptions? options = null)
    {
        var organization = Create(seed, options);

        db.Organizations.Add(organization);
        await db.SaveChangesAsync();

        return organization;
    }

    // Create x random organizations and add them to mock database.
    public override async Task<List<Organization>> SeedManyAsync(AppDbContext db, int seed, int count, List<OrganizationOptions>? options = null)
    {
        var organizations = CreateMany(seed, count, options);

        db.Organizations.AddRange(organizations);
        await db.SaveChangesAsync();

        return organizations;
    }

    // Create a random test organization using bogus.
    public override Faker<Organization> CreateFaker(int seed, OrganizationOptions? options = null)
    {
        return new Faker<Organization>()
            .UseSeed(seed)
            .RuleFor(o => o.Name, f => options is not null && !String.IsNullOrEmpty(options.OrganizationName) ? options.OrganizationName : f.Company.CompanyName());
    }
}