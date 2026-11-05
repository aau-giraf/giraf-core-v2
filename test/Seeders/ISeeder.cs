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