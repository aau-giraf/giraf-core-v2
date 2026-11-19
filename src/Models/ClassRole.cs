using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Models;

//Composite key, in order. Unique pairs
[PrimaryKey(nameof(ClassId), nameof(UserRoleId))]
public class ClassRole
{
    public required int ClassId { get; init; }
    public Class? Class { get; init; }

    public required int UserRoleId { get; init; }
    public UserRole? UserRole { get; init; }
}