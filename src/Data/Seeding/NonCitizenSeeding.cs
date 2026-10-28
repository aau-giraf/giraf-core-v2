namespace giraf_core_v2.Data.Seeding;

public class NonCitizenSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 3;
    public void Seed(AppDbContext context)
    {
        if (context.Set<NonCitizen>().Any()) return;

        NonCitizen[] nonCitizens = [
            new()
            {
                FirstName = "Mads", LastName = "Madsen",
                Username = "mads123", Password = BCrypt.Net.BCrypt.HashPassword("madskode", 12),
                Email = "mads@email.com"
            },
            new()
            {
                FirstName = "Lone", LastName = "Lonesen",
                Username = "lone123", Password = BCrypt.Net.BCrypt.HashPassword("lonekode", 12),
                Email = "lone@email.com"
            },
            new()
            {
                FirstName = "Sofie", LastName = "Sofiesen",
                Username = "sofie123", Password = BCrypt.Net.BCrypt.HashPassword("sofiekode", 12),
                Email = "sofie@email.com"
            }
        ];

        context.Set<NonCitizen>().AddRange(nonCitizens);
        context.SaveChanges();
    }
}