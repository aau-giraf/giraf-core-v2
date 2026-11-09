using Microsoft.EntityFrameworkCore;
namespace giraf_core_v2.Models;

//Configure a composite key in order.
[PrimaryKey(nameof(ClassId), nameof(EmployeeId))]
public class EmployeeClass
{
    public required int ClassId { get; init; }
    public Class? Class { get; init; }
    public required int EmployeeId { get; init; }
    public UserRole? Employee { get; init; }


}