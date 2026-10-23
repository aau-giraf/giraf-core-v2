using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Configure a composite key in order. Based on usage, introduce Id primary key if needed.
[PrimaryKey(nameof(NonCitizenId), nameof(Role), nameof(OrganizationId))]
public class UserRole
{
    public int NonCitizenId { get; init; }
    public NonCitizen? NonCitizen { get; init; }

    //The role
    public required RoleType Role { get; init; }

    //A role is in relation to an organization
    public required int OrganizationId { get; init; }
    public Organization? Organization { get; init; }
}

public enum RoleType
{
    //Based on use cases, further determine which needed enum values
    Employee, Parent
}

public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
{
    public void Configure(EntityTypeBuilder<UserRole> builder)
    {
        //Store RoleType enum as string, rather than the backing integer. Fragility if addition or reordering, e.g. Pedagogue added in the middle.
        builder.Property(userRole => userRole.Role)
            .HasConversion<string>();
    }
}