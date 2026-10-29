using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2._0.Endpoints;

public static class LegacyClassEndpoints
{
    public static void MapClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/class");


        group.MapPost("/create", CreateClassInOrganization);

    }

    private static async Task<Results<Ok<ResponseCreateClassDTO>, NotFound>> CreateClassInOrganization([FromForm] RequestCreateClassDTO requestCreateClassDTO, OrganizationService organizationservice)
    {
        var createdClass = await organizationservice.CreateClassInOrganizationAsync(requestCreateClassDTO, 0); //org ID is 0, because it will always be Egebakken
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }


}