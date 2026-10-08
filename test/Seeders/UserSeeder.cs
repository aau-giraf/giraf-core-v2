using BCrypt;


public static class UserSeeder
{
    // Create a random user and add it to mock database.
    public static async Task<User> SeedAsync(AppDbContext db, int seed)
    {
        var user = Create(seed);

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return user;
    }

    // Create x random users and add them to mock database.
    public static async Task<List<User>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var users = CreateMany(seed, count);

        db.Users.AddRange(users);
        await db.SaveChangesAsync();

        return users;
    }

    // Generate a random user.
    private static User Create(int seed)
    {
        return CreateFaker(seed).Generate();
    }

    // Create x random users.
    private static List<User> CreateMany(int seed, int count)
    {
        return CreateFaker(seed).Generate(count);
    }

    // Create a random test user using bogus.
    private static Faker<User> CreateFaker(int seed)
    {
        return new Faker<User>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, f => f.Internet.Email())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12));
    }
}
