enum UserType
{
    Citizen,
    NonCitizen
}

// Record to store DB user object and its unencrypted password.
internal record SeededUser(User User, string PlainPassword);
// Record of optional parameters used in user seeding.
// If UserOptions is not supplied, a random password will be generated and user type will default to citizen.
internal record UserOptions(string Password, UserType Type = UserType.Citizen);

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
        var users = CreateMany(seed, count, userOptions);

        // add all User classes to db.
        db.Users.AddRange(users.Select(x => x.User));
        await db.SaveChangesAsync();

        return users;
    }

    // Generate a random user.
    public SeededUser Create(int seed, UserOptions? userOptions = null)
    {
        UserOptions opt = OptionHandler(seed, userOptions);

        var user = CreateFaker(seed, opt).Generate();

        return new SeededUser(user, opt.Password);
    }

    // Create x random users.
    public List<SeededUser> CreateMany(
        int seed,
        int count,
        UserOptions? userOptions = null)
    {
        var users = new List<SeededUser>();
        // used to offset seed.
        int increment = 10;

        for (int i = 0; i < count; i++)
        {
            //offset seed to differ users.
            int offset = seed + (i * increment);
            UserOptions opt = OptionHandler(offset, userOptions);
            User user = CreateFaker(offset, opt).Generate();

            users.Add(new SeededUser(user, opt.Password));
        }

        return users;
    }

    // Create a random test user using bogus.
    public Faker<User> CreateFaker(int seed, UserOptions userOptions)
    {
        return new Faker<User>()
            .UseSeed(seed)
            .CustomInstantiator(f =>
            {
                var username = f.Internet.UserName();
                var firstName = f.Name.FirstName();
                var lastName = f.Name.LastName();
                var password = BCrypt.Net.BCrypt.HashPassword(
                    userOptions.Password, 12);

                User user = userOptions.Type switch
                {
                    UserType.Citizen => new Citizen
                    {
                        Username = username,
                        FirstName = firstName,
                        LastName = lastName,
                        Password = password,
                        ClassId = f.Random.Int(1, 10)
                    },

                    UserType.NonCitizen => new NonCitizen
                    {
                        Username = username,
                        FirstName = firstName,
                        LastName = lastName,
                        Password = password,
                        Email = f.Internet.Email()
                    },

                    _ => throw new ArgumentOutOfRangeException(
                        nameof(userOptions.Type))
                };

                return user;
            });
    }

    // Makes the options always usable by create faker
    private UserOptions OptionHandler(int seed, UserOptions? userOptions)
    {
        int minLength = 8;
        if (userOptions is null || userOptions.Password.Length < minLength)
        {
            var faker = new Faker<string>()
                .UseSeed(seed)
                .CustomInstantiator(f => f.Internet.Password());

            // ensure user type is not lost if supplied.
            userOptions = userOptions is not null && userOptions.Type == UserType.NonCitizen
                          ? new UserOptions(faker.Generate(), userOptions.Type)
                          : new UserOptions(faker.Generate());
        }
        return userOptions;
    }
}