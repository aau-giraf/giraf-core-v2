
public static class CitizenSeeder
{
    public record SeededCitizen(Citizen Citizen);


    public static async Task<Citizen> SeedAsync(AppDbContext db, int seed, int classId = 0)
    {
        var user = (await UserSeeder.SeedAsync(db, seed)).User;

        var citizen = new Citizen
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Password = user.Password,
            ClassId = classId
        };

        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return citizen;
    }
}
