using System.Diagnostics;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/", GetOrganizationsAsync);

        group.MapGet("/{organizationId:int}", GetOrganizationByIdAsync);

        group.MapGet("/{organizationId:int}/classes/{classId:int}", GetClassInOrganizationAsync);

        group.MapGet("/{organizationId:int}/classes", GetClassesInOrganizationAsync);

        group.MapPost("/{organizationId:int}/classes", CreateClassInOrganizationAsync);

        group.MapPost("/", CreateOrganizationAsync);

        group.MapPut("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", PutEmployeeRoleAsync);

        group.MapPatch("/{organizationId:int}", UpdateOrganizationAsync);

        group.MapDelete("/{organizationId:int}/members/{nonCitizenId:int}/{role:alpha}", DeleteEmployeeRoleAsync);

        group.MapDelete("/{organizationId:int}", DeleteOrganizationAsync);

        group.MapDelete("/{organizationId:int}/classes/{classId:int}", DeleteClassInOrganizationAsync);

    }

    private static async Task<Results<Ok<List<GetOrganizationResponseDTO>>, NotFound>> GetOrganizationsAsync(OrganizationService organizationService)
    {
        var organizations = await organizationService.GetOrganizationsAsync();
        return organizations == null ? TypedResults.NotFound() : TypedResults.Ok(organizations);
    }

    private static async Task<Results<Ok<GetOrganizationResponseDTO>, NotFound>> GetOrganizationByIdAsync(int organizationId, OrganizationService organizationService)
    {
        var organization = await organizationService.GetOrganizationByIdAsync(organizationId);
        return organization == null ? TypedResults.NotFound() : TypedResults.Ok(organization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrganizationAsync(int organizationId, OrganizationService organizationService)
    {
        var organization = await organizationService.DeleteOrganizationAsync(organizationId);
        return organization ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<GetClassInOrganizationResponseDTO>, NotFound>> GetClassInOrganizationAsync(int organizationId, int classId, OrganizationService organizationService)
    {
        var selectedClass = await organizationService.GetClassInOrganizationAsync(organizationId, classId);
        return selectedClass == null ? TypedResults.NotFound() : TypedResults.Ok(selectedClass);
    }

    private static async Task<Results<Ok<List<GetClassInOrganizationResponseDTO>>, NotFound>> GetClassesInOrganizationAsync(int organizationId, OrganizationService organizationService)
    {
        var classes = await organizationService.GetClassesInOrganizationAsync(organizationId);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrganizationAsync(int organizationId, int classId, OrganizationService organizationService)
    {
        var selectedClass = await organizationService.DeleteClassInOrganizationAsync(organizationId, classId);
        return selectedClass ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CreateClassResponseDTO>, NotFound>> CreateClassInOrganizationAsync([FromForm] CreateClassRequestDTO createClass, int organizationId, OrganizationService organizationService)
    {
        var createdClass = await organizationService.CreateClassInOrganizationAsync(createClass, organizationId);
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }

    private static async Task<Results<Ok<GetOrganizationResponseDTO>, NotFound>> CreateOrganizationAsync([FromForm] CreateOrganizationRequestDTO createOrganization, OrganizationService organizationService)
    {
        var createdOrganization = await organizationService.CreateOrganizationAsync(createOrganization);
        return createdOrganization == null ? TypedResults.NotFound() : TypedResults.Ok(createdOrganization);
    }

    private static async Task<Results<Ok<GetOrganizationResponseDTO>, NotFound>> UpdateOrganizationAsync([FromForm] CreateOrganizationRequestDTO updateOrganization, int organizationId, OrganizationService organizationService)
    {
        var updatedOrganization = await organizationService.UpdateOrganizationAsync(updateOrganization, organizationId);
        return updatedOrganization == null ? TypedResults.NotFound() : TypedResults.Ok(updatedOrganization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteEmployeeRoleAsync(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        var deletionOccurred = await organizationService.DeleteEmployeeRoleAsync(organizationId, nonCitizenId, role);
        return deletionOccurred ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<NotFound, NoContent, Created<PutEmployeeRoleResponseDTO>>> PutEmployeeRoleAsync(int organizationId, int nonCitizenId, RoleType role, OrganizationService organizationService)
    {
        (int, EmployeeRole?) status = await organizationService.CreateOrUpdateEmployeeRoleAsync(organizationId, nonCitizenId, role);
        return status switch
        {
            (-1, null) => TypedResults.NotFound(),
            (0, null) => TypedResults.NoContent(),
            (1, EmployeeRole employeeRole) => TypedResults.Created(
                $"/organizations/{organizationId}/members/{nonCitizenId}/{role}",
                new PutEmployeeRoleResponseDTO(employeeRole.NonCitizenId, employeeRole.OrganizationId, employeeRole.Role)),
            _ => throw new UnreachableException()
        };
    }
}