
public static class OrganizationEndpoints
{
    public static void MapClassEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/organizations");

        group.MapGet("/{org_id}/classes", GetClassesInOrginization);

        group.MapGet("/{org_id}", GetOrginizationById);
        
    }

    private static async Task<IResult> GetClassesInOrginization(OrganizationService service)
    {
        var result = await service.GetClassesInOrganizationAsync();
        return Results.Ok(classes);
    }

    private static async Task<IResult> GetOrginizationById(OrganizationService service)
    {   
        var result = await service.GetOrganizationByIdAsync(id, token, ct);
        return result.ToHttpResult(v => TypedResults.Ok(v));
    }

}