namespace giraf_core_v2.Tests.Seeders;

public static class CitizenSeeder
{
    // Record to store DB citizen object and the seeded users it is linked to.
    public record SeededCitizen(Citizen Citizen, UserSeeder.SeededUser User, UserSeeder.SeededUser Guardian);

    // Create a random citizen (with its user, guardian, organization and class) and add it to mock database.
    public static async Task<SeededCitizen> SeedAsync(AppDbContext db, int seed)
    {
        // citizen and guardian need different seeds, otherwise they get identical data
        var user = await UserSeeder.SeedAsync(db, seed);
        var guardian = await UserSeeder.SeedAsync(db, seed + 1);
        var schoolClass = await SeedClassAsync(db, seed);

        var citizen = Create(user.User, guardian.User, schoolClass);

        // add citizen to db
        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return new SeededCitizen(citizen, user, guardian);
    }

    // Create x random citizens in the same class with the same guardian and add them to mock database.
    public static async Task<List<SeededCitizen>> SeedManyAsync(AppDbContext db, int seed, int count, Class? schoolClass = null)
    {
        var users = await UserSeeder.SeedManyAsync(db, seed, count);
        var guardian = await UserSeeder.SeedAsync(db, seed + count);
        schoolClass ??= await SeedClassAsync(db, seed);

        var citizens = users
            .Select(u => new SeededCitizen(Create(u.User, guardian.User, schoolClass), u, guardian))
            .ToList();

        // add all Citizen classes to db.
        db.Citizens.AddRange(citizens.Select(x => x.Citizen));
        await db.SaveChangesAsync();

        return citizens;
    }

    // Generate a citizen linked to existing user, guardian and class.
    private static Citizen Create(User user, User guardian, Class schoolClass)
    {
        return new Citizen
        {
            UserId = user.Id,
            User = user,
            GuardianId = guardian.Id,
            Guardian = guardian,
            ClassId = schoolClass.Id,
            Class = schoolClass,
        };
    }

    // Create a random organization with a class and add them to mock database.
    public static async Task<Class> SeedClassAsync(AppDbContext db, int seed, Organization? organization = null)
    {
        var faker = new Faker { Random = new Randomizer(seed) };

        if (organization is null)
        {
            organization = new Organization { Name = faker.Company.CompanyName() };
            db.Organizations.Add(organization);
        }
        
        var schoolClass = new Class { Name = faker.Lorem.Word(), Organization = organization };

        db.Classes.Add(schoolClass);
        await db.SaveChangesAsync();

        return schoolClass;
    }
}
