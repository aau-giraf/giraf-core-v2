using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Models;

//Composite key, in order. Unique pairs
[PrimaryKey(nameof(ClassId), nameof(EmployeeRoleId))]
public class ClassEmployeeRole
{
    public required int ClassId { get; init; }
    public Class? Class { get; init; }

    public required int EmployeeRoleId { get; init; }
    public EmployeeRole? EmployeeRole { get; init; }
}