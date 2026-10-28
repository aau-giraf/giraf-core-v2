
public static class CitizenSeeder
{
    public record SeededCitizen(Citizen Citizen);


    public static async Task<Citizen> SeedCitizenAsync(AppDbContext db, int seed)
    {
        var user = (await UserSeeder.SeedAsync(db, seed)).User;
        /* Guardian and Class is for later implementation
        var guardian = (await UserSeeder.SeedAsync(db, seed + 1)).User;
        var girafClass = await ClassSeeder.SeedAsync(db, seed);
        */
        var citizen = new Citizen
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Password = user.Password,
        };

        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return citizen;
    }
}

