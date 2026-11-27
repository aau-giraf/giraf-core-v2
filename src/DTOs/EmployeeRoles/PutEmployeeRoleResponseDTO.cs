namespace giraf_core_v2.DTOs.EmployeeRoles;

public sealed record PutEmployeeRoleResponseDTO(
    int NonCitizenId,
    int OrganizationId,
    RoleType Role);