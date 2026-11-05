
public static class NonCitizenSeeder
{
    // Record to store DB user object and its unencrypted password.
    public record SeededNonCitizen(NonCitizen NonCitizen, string PlainPassword);

    // Create a random nonCitizen and add it to mock database.
    public static async Task<SeededNonCitizen> SeedAsync(AppDbContext db, int seed)
    {
        var nonCitizen = Create(seed);

        // add user to db
        db.NonCitizens.Add(nonCitizen.NonCitizen);
        await db.SaveChangesAsync();

        return nonCitizen;
    }

    // Create x random nonCitizens and add them to mock database.
    public static async Task<List<SeededNonCitizen>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var nonCitizens = CreateMany(seed, count);

        // add all User classes to db.
        db.Users.AddRange(nonCitizens.Select(x => x.NonCitizen));
        await db.SaveChangesAsync();

        return nonCitizens;
    }

    // Generate a random nonCitizens.
    private static SeededNonCitizen Create(int seed)
    {
        var faker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());

        var password = faker.Generate();

        var nonCitizen = CreateFaker(seed, password).Generate();

        return new SeededNonCitizen(nonCitizen, password); // returns hashed password in user and unhashed for itself
    }

    // Create x random nonCitizen.
    private static List<SeededNonCitizen> CreateMany(int seed, int count)
    {
        // create fakers to reuse for different data
        var NonCitizenFaker = new Faker<NonCitizen>()
                   .UseSeed(seed)
                   .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                   .RuleFor(u => u.LastName, f => f.Name.LastName())
                   .RuleFor(u => u.Username, f => f.Internet.UserName())
                   .RuleFor(u => u.Email, f => f.Internet.Email());

        var PasswordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var nonCitizens = new List<SeededNonCitizen>();

        for (int i = 0; i < count; i++)
        {
            var password = PasswordFaker.Generate();

            var nonCitizen = NonCitizenFaker.Generate();

            nonCitizen.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);

            nonCitizens.Add(new SeededNonCitizen(nonCitizen, password));
        }

        return nonCitizens;
    }

    // Create a random test nonCitizen using bogus.
    private static Faker<NonCitizen> CreateFaker(int seed, string password)
    {
        return new Faker<NonCitizen>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword(password, 12));
    }
}
