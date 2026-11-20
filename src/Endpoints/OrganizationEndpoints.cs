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
            (OrganizationServiceResult.ClassNotFound, null) => TypedResults.NotFound($"Class with ID {classId} not found."),
            (OrganizationServiceResult.Success, ClassResponseDTO classResponseDto) => TypedResults.Ok(classResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<Ok<List<ClassResponseDTO>>, NotFound<string>>> GetAllClassesInOrganization(int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult result, List<ClassResponseDTO>? classResponseDtoList) response = await organizationService.GetAllClassesInOrganizationAsync(organizationId);

        return response.result == OrganizationServiceResult.OrganizationNotFound ? TypedResults.NotFound($"Organization with ID {organizationId} not found.") : TypedResults.Ok(response.classResponseDtoList);
    }

    private static async Task<Results<Created<ClassResponseDTO>, NotFound<string>, Conflict<string>, InternalServerError>> CreateClassInOrganization([FromBody] CreateClassDTO createClassDto, int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult, ClassResponseDTO?) response = await organizationService.CreateClassInOrganizationAsync(createClassDto, organizationId);

        return response switch
        {
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.Success, ClassResponseDTO classResponseDto) => TypedResults.Created($"/organizations/{organizationId}/classes/{classResponseDto.ClassId}", classResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<Created<OrganizationResponseDTO>, Conflict<string>, InternalServerError>> CreateOrganization([FromBody] OrganizationDTO createOrganizationDto, OrganizationService organizationService)
    {
        (OrganizationServiceResult, OrganizationResponseDTO?) response = await organizationService.CreateOrganizationAsync(createOrganizationDto);

        return response switch
        {
            (OrganizationServiceResult.Success, OrganizationResponseDTO organizationResponseDto) => TypedResults.Created($"/organizations/{organizationResponseDto.OrganizationId}", organizationResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<NotFound<string>, Conflict<string>, Created<UserRoleResponseDTO>, InternalServerError>> AddRoleToUser(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        (OrganizationServiceResult, UserRoleResponseDTO?) response = await organizationService.AddRoleToUserAsync(organizationId, nonCitizenId, role);

        return response switch
        {
            (OrganizationServiceResult.UserNotFound, null) => TypedResults.NotFound($"User with ID {nonCitizenId} not found."),
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.UserRoleAlreadyExists, null) => TypedResults.Conflict($"Target user already has the proposed role."),
            (OrganizationServiceResult.Success, UserRoleResponseDTO userRoleResponseDto) => TypedResults.Created($"/organizations/{organizationId}/members/{nonCitizenId}/{role}", userRoleResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<Ok<OrganizationResponseDTO>, NotFound<string>, InternalServerError>> UpdateOrganization([FromBody] OrganizationDTO updateOrganizationDto, int organizationId, OrganizationService organizationService)
    {
        (OrganizationServiceResult, OrganizationResponseDTO?) response = await organizationService.UpdateOrganizationAsync(updateOrganizationDto, organizationId);

        return response switch
        {
            (OrganizationServiceResult.OrganizationNotFound, null) => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            (OrganizationServiceResult.Success, OrganizationResponseDTO organizationResponseDto) => TypedResults.Ok(organizationResponseDto),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<NoContent, NotFound<string>, InternalServerError>> DeleteRoleFromUser(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        OrganizationServiceResult response = await organizationService.DeleteUserRoleAsync(organizationId, nonCitizenId, role);

        return response switch
        {
            OrganizationServiceResult.UserNotFound => TypedResults.NotFound($"User with ID {nonCitizenId} not found."),
            OrganizationServiceResult.OrganizationNotFound => TypedResults.NotFound($"Organization with ID {organizationId} not found."),
            OrganizationServiceResult.UserRoleNotFound => TypedResults.NoContent(),
            OrganizationServiceResult.Success => TypedResults.NoContent(),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<NoContent, InternalServerError>> DeleteOrganization(int organizationId, OrganizationService organizationService)
    {
        OrganizationServiceResult response = await organizationService.DeleteOrganizationAsync(organizationId);

        return response switch
        {
            OrganizationServiceResult.OrganizationNotFound => TypedResults.NoContent(),
            OrganizationServiceResult.Success => TypedResults.NoContent(),
            _ => TypedResults.InternalServerError(),
        };
    }

    private static async Task<Results<NoContent, InternalServerError>> DeleteClassInOrganization(int organizationId, int classId, OrganizationService organizationService)
    {
        OrganizationServiceResult response = await organizationService.DeleteClassInOrganizationAsync(organizationId, classId);

        return response switch
        {
            OrganizationServiceResult.ClassNotFound => TypedResults.NoContent(),
            OrganizationServiceResult.Success => TypedResults.NoContent(),
            _ => TypedResults.InternalServerError(),
        };
    }
}