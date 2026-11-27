namespace giraf_core_v2.Data.Seeding;

public class EmployeeRoleSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 6;
    public void Seed(AppDbContext context)
    {
        if (context.Set<EmployeeRole>().Any()) return;

        NonCitizen[] nonCitizens = context.Set<NonCitizen>().ToArray();
        Organization[] organizations = context.Set<Organization>().ToArray();

        EmployeeRole[] employeeRoles = [
            new()
            {
                NonCitizenId = nonCitizens[0].Id, NonCitizen = nonCitizens[0],
                Role = RoleType.Employee,
                OrganizationId = organizations[0].Id, Organization = organizations[0]
            },
            new()
            {
                NonCitizenId = nonCitizens[1].Id, NonCitizen = nonCitizens[1],
                Role = RoleType.Employee,
                OrganizationId = organizations[0].Id, Organization = organizations[0]
            }
        ];

        context.Set<EmployeeRole>().AddRange(employeeRoles);
        context.SaveChanges();
    }
}