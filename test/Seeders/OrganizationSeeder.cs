// Options for creating org, if not supplied a random name is generated.
internal record OrgOptions(string Name = "");

internal class OrganizationSeeder : Seeder<Organization, OrgOptions>
{

    // Create a random organization and add it to mock database.
    public override async Task<Organization> SeedAsync(AppDbContext db, int seed, OrgOptions? opt = null)
    {
        var organization = Create(seed, opt);

        db.Organizations.Add(organization);
        await db.SaveChangesAsync();

        return organization;
    }

    // Create x random organizations and add them to mock database.
    public override async Task<List<Organization>> SeedManyAsync(AppDbContext db, int seed, int count, List<OrgOptions?>? opt = null)
    {
        var organizations = CreateMany(seed, count, opt);

        db.Organizations.AddRange(organizations);
        await db.SaveChangesAsync();

        return organizations;
    }

    // Create a random test organization using bogus.
    public override Faker<Organization> CreateFaker(int seed, OrgOptions? opt = null)
    {
        return new Faker<Organization>()
            .UseSeed(seed)
            .RuleFor(o => o.Name, f => opt is not null && !String.IsNullOrEmpty(opt.Name) ? opt.Name : f.Company.CompanyName());
    }
}