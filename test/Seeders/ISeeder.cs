// Common test seeder interface.
// TEntity -> database model to seed. (ex: User model used in the database)
// TReturn -> return type of seeded entities. (ex: SeededUser a record containing a user and its unencrypted password)
// TOption -> record containing all test specific parameters. (ex: User seeding can optionally be supplied with a password)
internal interface ISeeder<TEntity, TReturn, TOptions> where TEntity : class
{
    public Task<TReturn> SeedAsync(AppDbContext db, int seed, TOptions options);

    public Task<List<TReturn>> SeedManyAsync(
        AppDbContext db,
        int seed,
        int count,
        List<TOptions?> options
    );

    public TReturn Create(int seed, TOptions options);

    public List<TReturn> CreateMany(int seed, int count, List<TOptions?> options);

    public Faker<TEntity> CreateFaker(int seed, TOptions options);
}