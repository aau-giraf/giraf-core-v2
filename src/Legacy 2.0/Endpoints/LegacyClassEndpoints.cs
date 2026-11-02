using giraf_core_v2.Legacy_2._0.DTOs.Response;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2._0.Endpoints;

public static class LegacyClassEndpoints
{
    public static void MapLegacyClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/class");
        group.MapGet("/GetAll", GetAllClassrooms);

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


}