using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/", GetAllOrganizations);
        group.MapGet("/{orgId:int}", GetOrganizationById);
        group.MapGet("/{orgId:int}/classes/{classId:int}", GetClassInOrganization);

        group.MapGet("/{org_id:int}/classes", GetClassesInOrganization);

        group.MapPost("/{orgId:int}/classes", CreateClassInOrganization);

        group.MapPost("/", CreateOrganization);

        group.MapPut("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", PutUserRole);

        group.MapPatch("/{orgId:int}", UpdateOrganization);

        group.MapDelete("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", DeleteUserRole);

        group.MapDelete("/{orgId:int}", DeleteOrganization);

        group.MapDelete("/{orgId:int}/classes/{classId:int}", DeleteClassInOrganization);

    }

    private static async Task<Ok<List<OrganizationResponseDTO>>> GetAllOrganizations(OrganizationService organizationService)
    {
        List<OrganizationResponseDTO> organizations = await organizationService.GetAllOrganizationsAsync();

        return TypedResults.Ok(organizations);
    }

    private static async Task<Results<Ok<OrganizationResponseDTO>, NotFound<string>>> GetOrganizationById(int organizationId, OrganizationService organizationService)
    {
        var organization = await organizationService.GetOrganizationByIdAsync(organizationId); // TODO: ensart ved at bruge OrgServiceResult?

        return organization == null ? TypedResults.NotFound($"Organization with ID {organizationId} not found.") : TypedResults.Ok(organization);
    }

    private static async Task<Results<Ok<ClassResponseDTO>, NotFound<string>, InternalServerError>> GetClassInOrganization(int organizationId, int classId, OrganizationService organizationService)
    {
        (OrganizationServiceResult, ClassResponseDTO?) response = await organizationService.GetClassInOrganizationAsync(organizationId, classId);

        return response switch
        {
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.ClassNotFound, null) => TypedResults.NotFound($"Organization with ID {classId} not found."),
            (OrganizationServiceResult.Success, ClassResponseDTO classResponseDto) => TypedResults.Ok(classResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<Ok<List<ClassResponseDTO>>, NotFound<string>>> GetAllClassesInOrganization(int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult result, List<ClassResponseDTO>? classResponseDtoList) response = await organizationService.GetAllClassesInOrganizationAsync(organizationId);

        return response.result == OrganizationServiceResult.OrganizationNotFound ? TypedResults.NotFound($"Organization with ID {organizationId} not found.") : TypedResults.Ok(response.classResponseDtoList);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrganization(int orgId, int classId, OrganizationService organizationService)
    {
        var selectedClass = await organizationService.DeleteClassInOrganizationAsync(orgId, classId);
        return selectedClass ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseCreateClassDTO>, NotFound>> CreateClassInOrganization([FromForm] RequestCreateClassDTO requestCreateClassDTO, int orgId, OrganizationService organizationService)
    {
        var createdClass = await organizationService.CreateClassInOrganizationAsync(requestCreateClassDTO, orgId);
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }

    private static async Task<Results<Ok<ResponseGetOrganizationDTO>, NotFound>> CreateOrganization([FromForm] RequestCreateOrganizationDTO requestCreateOrganizationDTO, OrganizationService organizationService)
    {
        var createdOrganization = await organizationService.CreateOrganizationAsync(requestCreateOrganizationDTO);
        return createdOrganization == null ? TypedResults.NotFound() : TypedResults.Ok(createdOrganization);
    }

    private static async Task<Results<Ok<ResponseGetOrganizationDTO>, NotFound>> UpdateOrganization([FromForm] RequestCreateOrganizationDTO requestCreateOrganizationDTO, int orgId, OrganizationService organizationService)
    {
        var updatedOrganization = await organizationService.UpdateOrganizationAsync(requestCreateOrganizationDTO, orgId);
        return updatedOrganization == null ? TypedResults.NotFound() : TypedResults.Ok(updatedOrganization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUserRole(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        var deletionOccurred = await organizationService.DeleteUserRoleAsync(organizationId, nonCitizenId, role);
        return deletionOccurred ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<NotFound, NoContent, Created<UserRoleResponse>>> PutUserRole(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        (int, UserRole?) status = await organizationService.CreateOrUpdateUserRole(organizationId, nonCitizenId, role);
        return status switch
        {
            (-1, null) => TypedResults.NotFound(),
            (0, null) => TypedResults.NoContent(),
            (1, UserRole ur) => TypedResults.Created(
                $"/organizations/{organizationId}/members/{nonCitizenId}/{role}",
                new UserRoleResponse(ur.NonCitizenId, ur.OrganizationId, ur.Role)),
            _ => throw new UnreachableException()
        };
    }
}