namespace giraf_core_v2.Data.Seeding;

public class GuardianshipSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 5;
    public void Seed(AppDbContext context)
    {
        if (context.Set<Guardianship>().Any()) return;

        NonCitizen[] guardians = context.Set<NonCitizen>().ToArray();
        Citizen[] citizens = context.Set<Citizen>().ToArray();

        Guardianship[] guardianships = [
            new()
            {
                CitizenId = citizens[0].Id, Citizen = citizens[0],
                GuardianId = guardians[0].Id, Guardian = guardians[0]
            },
            new()
            {
                CitizenId = citizens[1].Id, Citizen = citizens[1],
                GuardianId = guardians[1].Id, Guardian = guardians[1]
            },
            new()
            {
                CitizenId = citizens[2].Id, Citizen = citizens[2],
                GuardianId = guardians[2].Id, Guardian = guardians[2]
            }
        ];

        context.Set<Guardianship>().AddRange(guardianships);
        context.SaveChanges();
    }
}