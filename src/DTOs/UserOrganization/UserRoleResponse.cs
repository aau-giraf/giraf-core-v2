public sealed record UserRoleResponse(
    int NonCitizenId,
    int OrganizationId,
    RoleType Role);