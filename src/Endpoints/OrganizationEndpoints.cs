using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/", GetOrganizations);

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

    private static async Task<Results<Ok<List<ResponseGetOrganizationDTO>>, NotFound>> GetOrganizations(OrganizationService organizationService)
    {
        var organizations = await organizationService.GetOrganizationsAsync();
        return organizations == null ? TypedResults.NotFound() : TypedResults.Ok(organizations);
    }

    private static async Task<Results<Ok<ResponseGetOrganizationDTO>, NotFound>> GetOrganizationById(int orgId, OrganizationService organizationService)
    {
        var organization = await organizationService.GetOrganizationByIdAsync(orgId);
        return organization == null ? TypedResults.NotFound() : TypedResults.Ok(organization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrganization(int orgId, OrganizationService organizationService)
    {
        var organization = await organizationService.DeleteOrganizationAsync(orgId);
        return organization ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseGetClassInOrganizationDTO>, NotFound>> GetClassInOrganization(int orgId, int classId, OrganizationService organizationService)
    {
        var selectedClass = await organizationService.GetClassInOrganizationAsync(orgId, classId);
        return selectedClass == null ? TypedResults.NotFound() : TypedResults.Ok(selectedClass);
    }

    private static async Task<Results<Ok<List<ResponseGetClassInOrganizationDTO>>, NotFound>> GetClassesInOrganization(int orgId, OrganizationService organizationService)
    {
        var classes = await organizationService.GetClassesInOrganizationAsync(orgId);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
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