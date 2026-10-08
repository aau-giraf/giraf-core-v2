using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Configure a composite key in order
[PrimaryKey(nameof(UserId), nameof(Type), nameof(OrganizationId))]
public class UserRole
{
    public int UserId {get; init; }
    public required User User { get; init; }

    //The role
    public RoleType Role { get; init; }

    //A role is in relation to an organization
    public required int OrganizationId {get; init;}
    public required Organization Organization { get; init; }
}

public enum RoleType
{
    Admin, Citizen, Teacher, Parent
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