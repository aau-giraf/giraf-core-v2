using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations").RequireAuthorization();

        // TODO: Add role requirements
        group.MapGet("/", GetAllOrganizations);
        group.MapGet("/{organizationId:int}", GetOrganizationById);
        group.MapGet("/{organizationId:int}/classes/{classId:int}", GetClassInOrganization);
        group.MapGet("/{organizationId:int}/classes", GetAllClassesInOrganization);
        group.MapPost("/{organizationId:int}/classes", CreateClassInOrganization);
        group.MapPost("/", CreateOrganization);
        group.MapPut("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", AddRoleToUser);
        group.MapPatch("/{organizationId:int}", UpdateOrganization);
        group.MapDelete("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", DeleteRoleFromUser);
        group.MapDelete("/{organizationId:int}", DeleteOrganization);
        group.MapDelete("/{organizationId:int}/classes/{classId:int}", DeleteClassInOrganization);
    }

    private static async Task<Ok<List<OrganizationResponseDTO>>> GetAllOrganizations(OrganizationService organizationService)
    {
        List<OrganizationResponseDTO> organizations = await organizationService.GetAllOrganizationsAsync();

        return TypedResults.Ok(organizations);
    }

    private static async Task<Results<Ok<OrganizationResponseDTO>, NotFound<string>, InternalServerError>> GetOrganizationById(int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult, OrganizationResponseDTO?) response = await organizationService.GetOrganizationByIdAsync(organizationId);

        return response switch
        {
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.Success, OrganizationResponseDTO organizationResponseDto) => TypedResults.Ok(organizationResponseDto),
            _ => TypedResults.InternalServerError(),
        };
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

    private static async Task<Results<Ok<ClassResponseDTO>, NotFound<string>, Conflict<string>, InternalServerError>> CreateClassInOrganization([FromBody] CreateClassDTO createClassDto, int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult, ClassResponseDTO?) response = await organizationService.CreateClassInOrganizationAsync(createClassDto, organizationId);

        return response switch
        {
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.ClassAlreadyExists, null) => TypedResults.Conflict($"Class with name '{createClassDto.Name.ToLower()}' already exists."),
            (OrganizationServiceResult.Success, ClassResponseDTO classResponseDto) => TypedResults.Ok(classResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<Ok<OrganizationResponseDTO>, Conflict<string>, InternalServerError>> CreateOrganization([FromBody] OrganizationDTO createOrganizationDto, OrganizationService organizationService)
    {
        (OrganizationServiceResult, OrganizationResponseDTO?) response = await organizationService.CreateOrganizationAsync(createOrganizationDto);

        return response switch
        {
            (OrganizationServiceResult.OrganizationAlreadyExists, null) => TypedResults.Conflict($"Organization with name '{createOrganizationDto.Name.ToLower()}' already exists."),
            (OrganizationServiceResult.Success, OrganizationResponseDTO organizationResponseDto) => TypedResults.Ok(organizationResponseDto),
            _ => TypedResults.InternalServerError(),
        };
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