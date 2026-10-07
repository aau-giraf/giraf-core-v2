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

        group.MapPost("/{org_id:int}/classes", CreateClassInOrganization);

        group.MapPost("/", CreateOrganization);

    }

    private static async Task<Results<Ok<List<ResponseGetOrganizationDTO>>, NotFound>> GetOrganizations(OrganizationService organizationService)
    {   
        var organizations = await organizationService.GetOrganizationsAsync();
        return organizations == null? TypedResults.NotFound() : TypedResults.Ok(organizations);
    }

    private static async  Task<Results<Ok<ResponseGetOrganizationDTO>, NotFound>> GetOrganizationById(int org_id, OrganizationService organizationService)
    {   
        var organization = await organizationService.GetOrganizationByIdAsync(org_id);
        return organization == null? TypedResults.NotFound() : TypedResults.Ok(organization);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteOrganization(int org_id, OrganizationService organizationservice)
    {
        var organization = await organizationservice.DeleteOrganizationAsync(org_id);
        return organization ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseGetClassInOrganizationDTO>, NotFound>> GetClassInOrganization(int org_id, int class_id, OrganizationService organizationservice)
    {
        var selectedClass = await organizationservice.GetClassInOrganizationAsync(org_id, class_id);
        return selectedClass == null ? TypedResults.NotFound() : TypedResults.Ok(selectedClass);
    }

    private static async Task<Results<Ok<List<ResponseGetClassInOrganizationDTO>>, NotFound>> GetClassesInOrganization(int org_id, OrganizationService organizationservice)
    {
        var classes = await organizationservice.GetClassesInOrganizationAsync(org_id);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrganization(int org_id, int class_id, OrganizationService organizationservice)
    {
        var selectedClass = await organizationservice.DeleteClassInOrganizationAsync(org_id, class_id);
        return selectedClass ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseCreateClassDTO>, NotFound>> CreateClassInOrganization([FromForm] RequestCreateClassDTO requestCreateClassDTO, int org_id, OrganizationService organizationservice)
    {   
        var createdClass = await organizationservice.CreateClassInOrganizationAsync(requestCreateClassDTO, org_id);
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }

     private static async Task<Results<Ok<ResponseGetOrganizationDTO>, NotFound>> CreateOrganization([FromForm] RequestCreateOrganizationDTO requestCreateOrganizationDTO, OrganizationService organizationservice)
    {   
        var createdOrganization = await organizationservice.CreateOrganizationAsync(requestCreateOrganizationDTO);
        return createdOrganization == null ? TypedResults.NotFound() : TypedResults.Ok(createdOrganization);
    }

}