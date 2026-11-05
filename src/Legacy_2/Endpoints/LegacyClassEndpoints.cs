using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using giraf_core_v2.Legacy_2.DTOs;

namespace giraf_core_v2.Legacy_2;

public static class LegacyClassEndpoints
{
    public static void MapLegacyClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/legacy/class");

        group.MapPost("/Create", CreateClassInOrganization);
        group.MapGet("/GetAll", GetAllClassrooms);
        group.MapDelete("/Delete/{classId}", DeleteClassInOrganization);

    }

    private static async Task<Results<Ok<ResponseCreateClassDTO>, NotFound>> CreateClassInOrganization([FromBody] LegacyRequestCreateClassDTO requestCreateClassDTO, OrganizationService organizationservice)
    {
        var requestCreateClass = new RequestCreateClassDTO { Name = requestCreateClassDTO.ClassName }; //Service vil geren have specifikke klasser så vil skal ændre dem hvis der skal ske noget andet 
        var createdClass = await organizationservice.CreateClassInOrganizationAsync(requestCreateClass, 1); //org ID is 1, because it will always be Egebakken
        return createdClass == null ? TypedResults.NotFound() : TypedResults.Ok(createdClass);
    }

    private static async Task<List<ClassroomResponseDTO>> GetAllClassrooms(OrganizationService organizationService)
    {
        // as of now, there only exists a single organization, so org_id = 1 is hardcoded. The tests for this method is structured to fail if there suddenly exists additional organizations
        var classrooms = await organizationService.GetClassesInOrganizationAsync(1);
        return classrooms.Select(c => new ClassroomResponseDTO
        {
            ClassId = c.Id,
            ClassName = c.Name
        }).ToList();
    }

    private static async Task<Results<NoContent, NotFound>> DeleteClassInOrganization(OrganizationService organizationService, int ClassId)
    {
        int OrgId = 1; // Currently only Egebakken

        bool result = await organizationService.DeleteClassInOrganizationAsync(OrgId, ClassId);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}