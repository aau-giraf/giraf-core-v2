using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;

public static class LegacyClassEndpoints
{
    public static void MapLegacyClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/legacy/class");

        group.MapPost("/create", CreateClassInOrganization);

    }

    private static async Task<Results<Ok<ResponseCreateClassDTO>, NotFound>> CreateClassInOrganization([FromForm] LegacyRequestCreateClassDTO requestCreateClassDTO, OrganizationService organizationservice)
    {
        var requestCreateClass = new RequestCreateClassDTO{Name = requestCreateClassDTO.ClassName}; //Service vil geren have specifikke klasser så vil skal ændre dem hvis der skal ske noget andet 
        var createdClass = await organizationservice.CreateClassInOrganizationAsync(requestCreateClass, 1); //org ID is 0, because it will always be Egebakken
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }




}