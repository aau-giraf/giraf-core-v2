using BCrypt;


public static class UserSeeder
{
    // Record to store DB user object and its unencrypted password.
    public record SeededUser(User User, string PlainPassword);

    // Create a random user and add it to mock database.
    public static async Task<SeededUser> SeedAsync(AppDbContext db, int seed)
    {
        var user = Create(seed);

        // add user to db
        db.Users.Add(user.User);
        await db.SaveChangesAsync();

        return user;
    }

    // Create x random users and add them to mock database.
    public static async Task<List<SeededUser>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var users = CreateMany(seed, count);

        // add all User classes to db.
        db.Users.AddRange(users.Select(x => x.User));
        await db.SaveChangesAsync();

        return users;
    }

    // Generate a random user.
    private static SeededUser Create(int seed)
    {
        var faker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());

        var password = faker.Generate();

        var user = CreateFaker(seed, password).Generate();

        return new SeededUser(user, password);
    }

    // Create x random users.
    private static List<SeededUser> CreateMany(int seed, int count)
    {
        // create fakers to reuse for different data
        var UserFaker = new Faker<User>()
                   .UseSeed(seed)
                   .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                   .RuleFor(u => u.LastName, f => f.Name.LastName())
                   .RuleFor(u => u.Email, f => f.Internet.Email())
                   .RuleFor(u => u.Username, f => f.Internet.UserName());

        var PasswordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var users = new List<SeededUser>();

        for (int i = 0; i < count; i++)
        {
            var password = PasswordFaker.Generate();

            var user = UserFaker.Generate();

            user.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);

            users.Add(new SeededUser(user, password));
        }

        return users;
    }

    // Create a random test user using bogus.
    private static Faker<User> CreateFaker(int seed, string password)
    {
        return new Faker<User>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword(password, 12));
    }
}
