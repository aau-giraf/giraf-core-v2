
public static class OrganizationEndpoints
{
    public static void MapClassEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/{org_id:int}/classes", GetClassesInOrginization);

        group.MapGet("/{org_id:int}", GetOrginizationById);
        
    }


    private static async Task<IResult> GetClassesInOrginization(int org_id, OrganizationService organizationservice)
    {
        var classes = await organizationservice.GetClassesInOrganizationAsync(org_id);
        
        return Results.Ok(classes);
    }


    private static async Task<IResult> GetOrginizationById(int org_id, OrganizationService organizationService)
    {   
        var organization = await organizationService.GetOrganizationByIdAsync(org_id);
        return Results.Ok(organization);
    }

}