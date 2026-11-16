
public static class CitizenSeeder
{
    public record SeededCitizen(Citizen Citizen);


    public static async Task<Citizen> SeedAsync(AppDbContext db, int seed, int classId = 0)
    {
        // Insert a single Citizen. Copying a saved user's username fails the unique index on Postgres.
        var citizen = new Faker<Citizen>()
            .UseSeed(seed)
            .CustomInstantiator(f => new Citizen
            {
                FirstName = f.Name.FirstName(),
                LastName = f.Name.LastName(),
                Username = f.Internet.UserName(),
                Password = BCrypt.Net.BCrypt.HashPassword(f.Internet.Password(), 12),
                ClassId = classId
            })
            .Generate();

        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return citizen;
    }
}