
public static class CitizenSeeder
{
    // Record to store DB user object and its unencrypted password.
    public record SeededCitizen(Citizen citizen);

    // Create a random user and add it to mock database.
    public static async Task<SeededCitizen> SeedAsync(AppDbContext db, int seed, Class girafClass)
    {
        var citizen = Create(seed, girafClass);

        // add user to db
        db.Users.Add(citizen.citizen);
        await db.SaveChangesAsync();

        return citizen;
    }

    // Create x random users and add them to mock database.
    public static async Task<List<SeededCitizen>> SeedManyAsync(AppDbContext db, int seed, int count, IReadOnlyList<Class> classes)
    {
        var citizens = CreateMany(seed, count, classes);

        // add all User classes to db.
        db.Users.AddRange(citizens.Select(x => x.citizen));
        await db.SaveChangesAsync();

        return citizens;
    }

    // Generate a random user.
    public static SeededCitizen Create(int seed, Class girafClass)
    {
        var password = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password()).Generate();
        var citizen = CreateFaker(seed, [girafClass]).Generate();

        citizen.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);
        return new SeededCitizen(citizen); // returns hashed password in user and unhashed for itself
    }

    // Create x random users.
    private static List<SeededCitizen> CreateMany(int seed, int count, IReadOnlyCollection<Class> classes)
    {
        // create fakers to reuse for different data
        var citizenFaker = CreateFaker(seed, classes);
        var passwordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var citizens = new List<SeededCitizen>();

        for (int i = 0; i < count; i++)
        {
            var password = passwordFaker.Generate();
            var citizen = citizenFaker.Generate();

            citizen.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);
            citizens.Add(new SeededCitizen(citizen));
        }

        return citizens;
    }

    // Create a random test user using bogus.
    private static Faker<Citizen> CreateFaker(int seed, IReadOnlyCollection<Class> classes)
    {
        return new Faker<Citizen>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.ClassId, f => f.PickRandom<Class>(classes).Id);
    }
}