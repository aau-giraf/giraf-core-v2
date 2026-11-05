// Record to store DB user object and its unencrypted password.
internal record SeededUser(User User, string PlainPassword);
// Record of optional parameters used in user seeding.
// If UserOptions is not supplied, a random password will be generated.
internal record UserOptions(string Password);

internal class UserSeeder : ISeeder<User, SeededUser, UserOptions>
{
    // Create a random user and add it to mock database.
    public async Task<SeededUser> SeedAsync(AppDbContext db, int seed, UserOptions? userOptions = null)
    {
        var user = Create(seed, userOptions);

        // add user to db
        db.Users.Add(user.User);
        await db.SaveChangesAsync();

        return user;
    }

    // Create x random users and add them to mock database.
    public async Task<List<SeededUser>> SeedManyAsync(AppDbContext db, int seed, int count, UserOptions? userOptions = null)
    {
        var users = CreateManyFakers(seed, count, userOptions);

        // add all User classes to db.
        db.Users.AddRange(users.Select(x => x.User));
        await db.SaveChangesAsync();

        return users;
    }

    // Generate a random user.
    public SeededUser Create(int seed, UserOptions? userOptions = null)
    {
        User user;
        string password;
        if (userOptions is null)
        {
            var faker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());

            password = faker.Generate();
            UserOptions opt = new(password);
            user = CreateFaker(seed, opt);
        }
        else
        {
            user = CreateFaker(seed, userOptions).Generate();
            password = userOptions.Password;
        }

        return new SeededUser(user, password); // returns hashed password in user and unhashed for itself
    }

    // Create x random users.
    public List<SeededUser> CreateManyFakers(int seed, int count, UserOptions? userOptions = null)
    {
        // create fakers to reuse for different data
        var userFaker = new Faker<User>()
                   .UseSeed(seed)
                   .RuleFor(u => u.FirstName, f => f.Name.FirstName())
                   .RuleFor(u => u.LastName, f => f.Name.LastName())
                   .RuleFor(u => u.Username, f => f.Internet.UserName());

        var passwordFaker = new Faker<string>().UseSeed(seed).CustomInstantiator(f => f.Internet.Password());
        var users = new List<SeededUser>();

        for (int i = 0; i < count; i++)
        {
            var password = passwordFaker.Generate();

            var user = userFaker.Generate();

            user.Password = BCrypt.Net.BCrypt.HashPassword(password, 12);

            users.Add(new SeededUser(user, password));
        }

        return users;
    }

    // Create a random test user using bogus.
    public Faker<User> CreateFaker(int seed, UserOptions userOptions)
    {
        return new Faker<User>()
            .UseSeed(seed)
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Username, f => f.Internet.UserName())
            .RuleFor(u => u.Password, f => BCrypt.Net.BCrypt.HashPassword(userOptions.Password, 12));
    }
}