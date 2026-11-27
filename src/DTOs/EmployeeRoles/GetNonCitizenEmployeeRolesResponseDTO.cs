using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.EmployeeRoles;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class GetNonCitizenEmployeeRolesResponseDTO
{
    public required int NonCitizenId { get; set; }
    public required ICollection<string> RoleOrganizationTuples { get; set; } = [];

}