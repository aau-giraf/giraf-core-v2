using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

public static class OrganizationEndpoints
{
    public static void MapOrganizationEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/", GetOrganizations);

        group.MapGet("/{org_id:int}", GetOrganizationById);

        group.MapDelete("/{org_id:int}", DeleteOrganization);

        group.MapGet("/{org_id:int}/classes/{class_id:int}", GetClassInOrganization);

        group.MapGet("/{org_id:int}/classes", GetClassesInOrganization);

        group.MapDelete("/{org_id:int}/classes/{class_id:int}", DeleteClassInOrganization);

        group.MapPost("/{org_id}/classes", CreateClassInOrganization);

    }

    private static async Task<Results<Ok<List<Organization>>, NotFound>> GetOrganizations(OrganizationService organizationService)
    {   
        var organizations = await organizationService.GetOrganizationsAsync();
        return organizations == null? TypedResults.NotFound() : TypedResults.Ok(organizations);
    }

    private static async  Task<Results<Ok<Organization>, NotFound>> GetOrganizationById(int org_id, OrganizationService organizationService)
    {   
        var organization = await organizationService.GetOrganizationByIdAsync(org_id);
        return organization == null? TypedResults.NotFound() : TypedResults.Ok(organization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrganization(int org_id, OrganizationService organizationservice)
    {
        var organization = await organizationservice.DeleteOrganizationAsync(org_id);
        return organization ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<Class>, NotFound>> GetClassInOrganization(int org_id, int class_id, OrganizationService organizationservice)
    {
        var oneClass = await organizationservice.GetClassInOrganizationAsync(org_id, class_id);
        return oneClass == null ? TypedResults.NotFound() : TypedResults.Ok(oneClass);
    }

    private static async Task<Results<Ok<List<Class>>, NotFound>> GetClassesInOrganization(int org_id, OrganizationService organizationservice)
    {
        var classes = await organizationservice.GetClassesInOrganizationAsync(org_id);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrganization(int org_id, int class_id, OrganizationService organizationservice)
    {
        var oneClass = await organizationservice.DeleteClassInOrganizationAsync(org_id, class_id);
        return oneClass ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<Class>, NotFound>> CreateClassInOrganization([FromForm] string className , [FromForm] int org_id, OrganizationService organizationservice)
    {   
        var createdClass = await organizationservice.CreateClassInOrganizationAsync(className, org_id);
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }

}