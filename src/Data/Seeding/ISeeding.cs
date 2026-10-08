namespace giraf_core_v2.Data.Seeding;

public interface ISeeding
{
    public abstract void Seed(AppDbContext context);

    public int SeedingPosition { get; init;}
}