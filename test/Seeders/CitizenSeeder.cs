
public static class CitizenSeeder
{
    // Record to store DB user object and its unencrypted password.
    public record SeededCitizen(Citizen citizen, string PlainPassword);

    // Create a random user and add it to mock database.
    public static async Task<SeededCitizen> SeedAsync(AppDbContext db, int seed)
    {
        var citizen = Create(seed);

        // add user to db
        db.Users.Add(citizen.citizen);
        await db.SaveChangesAsync();

        return citizen;
    }

    // Create x random users and add them to mock database.
    public static async Task<List<SeededCitizen>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var citizens = CreateMany(seed, count);

        // add all User classes to db.
        db.Users.AddRange(citizens.Select(x => x.citizen));
        await db.SaveChangesAsync();

        return citizens;
    }

    // Generate a random user.
    private static SeededCitizen Create(int seed)
    {
        var faker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());

        var password = faker.Generate();

        var citizen = CreateFaker(seed, password).Generate();

        return new SeededCitizen(citizen, password); // returns hashed password in user and unhashed for itself
    }

    // Create x random users.
    private static List<SeededCitizen> CreateMany(int seed, int count)
    {
        // create fakers to reuse for different data
        var CitizenFaker = new Faker<Citizen>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.ClassId, f => f.Random.Number(1, 100));

        var PasswordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var citizens = new List<SeededCitizen>();

        for (int i = 0; i < count; i++)
        {
            var password = PasswordFaker.Generate();

            var citizen = CitizenFaker.Generate();

            citizen.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);

            citizens.Add(new SeededCitizen(citizen, password));
        }

        return citizens;
    }

    // Create a random test user using bogus.
    private static Faker<Citizen> CreateFaker(int seed, string password)
    {
        return new Faker<Citizen>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.ClassId, f => f.Random.Number(1, 100))
            .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword(password, 12));
    }
}