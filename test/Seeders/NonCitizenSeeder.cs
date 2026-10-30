public static class NonCitizenSeeder
{
    // Record to store DB nonCitizen object and its unencrypted password.
    public record SeededNonCitizen(NonCitizen NonCitizen, string PlainPassword);

    // Create a random nonCitizen and add it to mock database.
    public static async Task<SeededNonCitizen> SeedAsync(AppDbContext db, int seed)
    {
        var nonCitizen = Create(seed);

        // add nonCitizen to db
        db.NonCitizens.Add(nonCitizen.NonCitizen);
        await db.SaveChangesAsync();

        return nonCitizen;
    }

    // Create x random nonCitizens and add them to mock database.
    public static async Task<List<SeededNonCitizen>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var nonCitizens = CreateMany(seed, count);

        // add all nonCitizen classes to db.
        db.NonCitizens.AddRange(nonCitizens.Select(x => x.NonCitizen));
        await db.SaveChangesAsync();

        return nonCitizens;
    }

    // Generate a random nonCitizen.
    private static SeededNonCitizen Create(int seed)
    {
        var faker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());

        var password = faker.Generate();

        var nonCitizen = CreateFaker(seed, password).Generate();

        return new SeededNonCitizen(nonCitizen, password); // returns hashed password in nonCitizen and unhashed for itself
    }

    // Create x random nonCitizens.
    private static List<SeededNonCitizen> CreateMany(int seed, int count)
    {
        // create fakers to reuse for different data
        var UserFaker = new Faker<NonCitizen>()
                   .UseSeed(seed)
                   .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                   .RuleFor(u => u.LastName, f => f.Name.LastName())
                   .RuleFor(u => u.Username, f => f.Internet.UserName())
                   .RuleFor(u => u.Email, ""); // email field is 'required', but not needed

        var PasswordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var users = new List<SeededNonCitizen>();

        for (int i = 0; i < count; i++)
        {
            var password = PasswordFaker.Generate();

            var nonCitizen = UserFaker.Generate();

            nonCitizen.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);

            users.Add(new SeededNonCitizen(nonCitizen, password));
        }

        return users;
    }

    // Create a random test nonCitizen using bogus.
    private static Faker<NonCitizen> CreateFaker(int seed, string password)
    {
        return new Faker<NonCitizen>()
            .UseSeed(seed)
            .RuleFor(nc => nc.FirstName, f => f.Name.FirstName())
            .RuleFor(nc => nc.LastName, f => f.Name.LastName())
            .RuleFor(nc => nc.Username, f => f.Internet.UserName())
            .RuleFor(nc => nc.Email, f => f.Internet.Email())
            .RuleFor(nc => nc.Password, f => BCrypt.Net.BCrypt.HashPassword(password, 12));
    }
}