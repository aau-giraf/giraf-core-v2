namespace giraf_core_v2.Data.Seeding;

public class CitizenSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 4;
    public void Seed(AppDbContext context)
    {
        if (context.Set<Citizen>().Any()) return;

        User[] users = context.Set<User>().ToArray();

        Class class1 = context.Set<Class>().First(class_ => class_.Name == "3.Y");
        Class class2 = context.Set<Class>().First(class_ => class_.Name == "4.B");

        Citizen[] citizens = [
            new()
            {
                FirstName = "Mikkel", LastName = "Mikkelsen",
                Username = "mikkel123", Password = BCrypt.Net.BCrypt.HashPassword("mikkelkode", 12),
                Class = class1
            },
            new()
            {
                FirstName = "Erik", LastName = "Eriksen",
                Username = "erik123", Password = BCrypt.Net.BCrypt.HashPassword("erikkode", 12),
                Class = class1
            },
            new()
            {
                FirstName = "Hans", LastName = "Hansen",
                Username = "hans123", Password = BCrypt.Net.BCrypt.HashPassword("hanskode", 12),
                Class = class2
            }
        ];

        context.Set<Citizen>().AddRange(citizens);
        context.SaveChanges();
    }
}