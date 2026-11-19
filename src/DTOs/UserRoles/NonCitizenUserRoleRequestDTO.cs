using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.UserRoles;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class NonCitizenUserRoleRequestDTO
{
    public required RoleType Role { get; set; }

    public required int OrganizationId { get; set; }

}