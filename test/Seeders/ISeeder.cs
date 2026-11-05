// Common test seeder interface.
// TEntity -> database model to seed. (ex: User model used in the database)
// TReturn -> return type of seeded entities. (ex: SeededUser a record containing a user and its unencrypted password)
// TOption -> record containing all test specific parameters. (ex: User seeding can optionally be supplied with a password)
internal interface ISeeder<TEntity, TReturn, TOptions> where TEntity : class
{
    Task<TReturn> SeedAsync(AppDbContext db, int seed, TOptions options);

    Task<List<TReturn>> SeedManyAsync(
        AppDbContext db,
        int seed,
        int count,
        TOptions options
    );

    TReturn Create(int seed, TOptions options);

    List<TReturn> CreateManyFakers(int seed, int count, TOptions options);

    Faker<TEntity> CreateFaker(int seed, TOptions options);
}