using Microsoft.AspNetCore.Http.HttpResults;

public static class OrganizationEndpoints
{
    public static void MapClassEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/", GetOrginizations);

        group.MapGet("/{org_id:int}", GetOrginizationById);

        group.MapDelete("/{org_id:int}", DeleteOrginization);

        group.MapGet("/{org_id:int}/classes/{class_id:int}", GetClassInOrginization);

        group.MapGet("/{org_id:int}/classes", GetClassesInOrginization);

        group.MapDelete("/{org_id:int}/classes/{class_id:int}", DeleteClassInOrginization);

    }

    private static async Task<Results<Ok<List<Organization>>, NotFound>> GetOrginizations(OrganizationService organizationService)
    {   
        var organizations = await organizationService.GetOrganizationsAsync();
        return organizations == null? TypedResults.NotFound() : TypedResults.Ok(organizations);
    }

    private static async  Task<Results<Ok<Organization>, NotFound>> GetOrginizationById(int org_id, OrganizationService organizationService)
    {   
        var organization = await organizationService.GetOrganizationByIdAsync(org_id);
        return organization == null? TypedResults.NotFound() : TypedResults.Ok(organization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrginization(int org_id, OrganizationService organizationservice)
    {
        var organization = await organizationservice.DeleteOrganizationAsync(org_id);
        return organization ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<Class>, NotFound>> GetClassInOrginization(int org_id,int class_id, OrganizationService organizationservice)
    {
        var oneClass = await organizationservice.GetClassInOrganizationAsync(org_id, class_id);
        return oneClass == null ? TypedResults.NotFound() : TypedResults.Ok(oneClass);
    }

    private static async Task<Results<Ok<List<Class>>, NotFound>> GetClassesInOrginization(int org_id, OrganizationService organizationservice)
    {
        var classes = await organizationservice.GetClassesInOrganizationAsync(org_id);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrginization(int org_id,int class_id, OrganizationService organizationservice)
    {
        var oneClass = await organizationservice.DeleteClassInOrganizationAsync(org_id, class_id);
        return oneClass ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}