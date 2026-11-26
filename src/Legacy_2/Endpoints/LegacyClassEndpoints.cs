using giraf_core_v2.Legacy_2.DTOs;

using Microsoft.AspNetCore.Http.HttpResults;

namespace giraf_core_v2.Legacy_2;

public static class LegacyClassEndpoints
{
    public static void MapLegacyClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/legacy/foodplanner/api/Classrooms");

        group.MapGet("/GetAll", GetAllClassrooms);
        group.MapDelete("/Delete/{classId}", DeleteClassInOrganization);
    }

    private static async Task<List<ClassroomResponseDTO>> GetAllClassrooms(OrganizationService organizationService)
    {
        // as of now, there only exists a single organization, so org_id = 1 is hardcoded.
        var classrooms = await organizationService.GetClassesInOrganizationAsync(1);
        return classrooms.Select(c => new ClassroomResponseDTO
        {
            ClassId = c.ClassId,
            ClassName = c.Name
        }).ToList();
    }

    private static async Task<Results<Ok<int>, BadRequest>> DeleteClassInOrganization(OrganizationService organizationService, int ClassId)
    {
        int orgId = 1; // Currently only Egebakken

        bool result = await organizationService.DeleteClassInOrganizationAsync(orgId, ClassId);
        int response = (result == true ? 1 : 0); // Foodplanner returns repository delete method response as body: > 0 = success, <= 0 = failure
        return result ? TypedResults.Ok(response) : TypedResults.BadRequest();
    }
}