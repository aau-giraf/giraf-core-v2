// Common seeder for test seeders requiring special return types.
// TEntity -> database model to seed. (ex: User model used in the database)
// TResult -> Custom return type of seeded entities. (ex: SeededUser a record containing a user and its unencrypted password)
// TOption -> record containing all test specific parameters. (ex: User seeding can optionally be supplied with a password)
internal abstract class CustomResultSeeder<TEntity, TReturn, TOptions> where TEntity : class
{
    // Create a random TEntity or use supplied options and add it to mock database.
    // Return the entity with custom result object.
    public abstract Task<TReturn> SeedAsync(AppDbContext db, int seed, TOptions options);

    // Create x random TEntity or use supplied options and add it to mock database.
    // Return the list of entities with custom result object.
    public abstract Task<List<TReturn>> SeedManyAsync(
        AppDbContext db,
        int seed,
        int count,
        List<TOptions> options
    );

    // Generate a TEntity and return it as a custom result object.
    public abstract TReturn Create(int seed, TOptions options);

    // Generate x TEntity based on options or randomly and return as list of custom result objects
    public abstract List<TReturn> CreateMany(int seed, int count, List<TOptions> options);

    // Create a bogus faker with random content or based on optios.
    public abstract Faker<TEntity> CreateFaker(int seed, TOptions options);
}