// Base test class for creating in memory db for tests.
using Xunit.Abstractions;

public abstract class TestBase
{

    protected int Seed { get; }

    // Create a seed for the tests
    protected TestBase(ITestOutputHelper output)
    {
        Seed = Random.Shared.Next();
        output.WriteLine($"Test seed: {Seed}");
    }

    // Create a mock database.
    protected AppDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }



}
