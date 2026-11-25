namespace giraf_core_v2.DTOs.UserOrganization;

public sealed record PutUserRoleResponseDTO(
    int NonCitizenId,
    int OrganizationId,
    RoleType Role);