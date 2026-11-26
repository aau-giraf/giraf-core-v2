using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

public class EmployeeRole
{
    public int Id { get; init; }

    public int NonCitizenId { get; init; }
    public NonCitizen? NonCitizen { get; init; }

    //A role is in relation to an organization
    public required int OrganizationId { get; init; }
    public Organization? Organization { get; init; }

    //The role
    public required RoleType Role { get; init; }

    //Navigational property: class assignments involving this organizational role
    public ICollection<ClassEmployeeRole> ClassRoles { get; } = [];
}

public enum RoleType
{
    //Based on use cases, further determine which needed enum values
    Employee
}

public class EmployeeRoleConfiguration : IEntityTypeConfiguration<EmployeeRole>
{
    public void Configure(EntityTypeBuilder<EmployeeRole> builder)
    {
        //Store RoleType enum as string, rather than the backing integer. Fragility if addition or reordering, e.g. Pedagogue added in the middle.
        builder.Property(userRole => userRole.Role)
            .HasConversion<string>();

        //Alternate composite key - Prevent duplicate (non-citizen, role, organization) tuples
        builder.HasAlternateKey(userRole => new { userRole.NonCitizenId, userRole.Role, userRole.OrganizationId });

    }
}