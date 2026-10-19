namespace giraf_core_v2.Data.Seeding;

public class UserRoleSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 6;
    public void Seed(AppDbContext context)
    {
        if (context.Set<UserRole>().Any()) return;

        NonCitizen[] nonCitizens = context.Set<NonCitizen>().ToArray();
        Organization[] organizations = context.Set<Organization>().ToArray();

        UserRole[] userRoles = [
            new()
            {
                NonCitizenId = nonCitizens[0].Id, NonCitizen = nonCitizens[0],
                Role = RoleType.Employee,
                OrganizationId = organizations[0].Id, Organization = organizations[0]
            },
            new()
            {
                NonCitizenId = nonCitizens[1].Id, NonCitizen = nonCitizens[1],
                Role = RoleType.Parent,
                OrganizationId = organizations[0].Id, Organization = organizations[0]
            },
            new()
            {
                NonCitizenId = nonCitizens[2].Id, NonCitizen = nonCitizens[2],
                Role = RoleType.Parent,
                OrganizationId = organizations[1].Id, Organization = organizations[1]
            }
        ];

        context.Set<UserRole>().AddRange(userRoles);
        context.SaveChanges();
    }
}
