// Record of optional parameters. Non-positive numbers result in random postive integer.
namespace giraf_core_v2.Tests.Seeders;

internal record ClassOptions(string ClassName = "", int OrganizationId = -9999);

internal class ClassSeeder : Seeder<Class, ClassOptions>
{
    // Create a random class and add it to mock database.
    public override async Task<Class> SeedAsync(AppDbContext db, int seed, ClassOptions? options = null)
    {
        var schoolClass = Create(seed, options);

        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();

        return schoolClass;
    }

    // Create x random classes and add them to mock database.
    public override async Task<List<Class>> SeedManyAsync(AppDbContext db, int seed, int count, List<ClassOptions>? options = null)
    {
        var classes = CreateMany(seed, count, options);

        db.Classes.AddRange(classes);
        await db.SaveChangesAsync();

        return classes;
    }

    // Create a random test class using bogus.
    public override Faker<Class> CreateFaker(int seed, ClassOptions? options = null)
    {
        return new Faker<Class>()
            .UseSeed(seed)
            .RuleFor(c => c.Name, f => options is not null && !String.IsNullOrEmpty(options.ClassName) ? options.ClassName : f.Commerce.Department())
            .RuleFor(c => c.OrganizationId, f => options is not null && options.OrganizationId > 0 ? options.OrganizationId : f.Random.Int(1, Int32.MaxValue));
    }

}