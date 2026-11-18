// Record of optional parameters. Non-positive numbers result in random postive integer.
internal record ClassOptions(string ClassName = "", int OrgId = -9999);

internal class ClassSeeder : Seeder<Class, ClassOptions>
{
    // Create a random class and add it to mock database.
    public override async Task<Class> SeedAsync(AppDbContext db, int seed, ClassOptions? opt = null)
    {
        var class_ = Create(seed, opt);

        db.Classes.Add(class_);
        await db.SaveChangesAsync();

        return class_;
    }

    // Create x random classes and add them to mock database.
    public override async Task<List<Class>> SeedManyAsync(AppDbContext db, int seed, int count, List<ClassOptions>? opt = null)
    {
        var classes = CreateMany(seed, count, opt);

        db.Classes.AddRange(classes);
        await db.SaveChangesAsync();

        return classes;
    }

    // Create a random test class using bogus.
    public override Faker<Class> CreateFaker(int seed, ClassOptions? opt = null)
    {
        return new Faker<Class>()
            .UseSeed(seed)
            .RuleFor(c => c.Name, f => opt is not null && !String.IsNullOrEmpty(opt.ClassName) ? opt.ClassName : f.Commerce.Department())
            .RuleFor(c => c.OrganizationId, f => opt is not null && opt.OrgId > 0 ? opt.OrgId : f.Random.Int(1, Int32.MaxValue));
    }

}