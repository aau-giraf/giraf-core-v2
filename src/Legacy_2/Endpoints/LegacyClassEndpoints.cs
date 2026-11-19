using giraf_core_v2.Legacy_2.DTOs;

namespace giraf_core_v2.Legacy_2;

public static class LegacyClassEndpoints
{
    public static void MapLegacyClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/legacy/foodplanner/api/Classrooms");

        group.MapGet("/GetAll", GetAllClassrooms);
    }

    private static async Task<List<ClassroomResponseDTO>> GetAllClassrooms(OrganizationService organizationService)
    {
        // as of now, there only exists a single organization, so org_id = 1 is hardcoded.
        var classrooms = await organizationService.GetClassesInOrganizationAsync(1);
        return classrooms.Select(c => new ClassroomResponseDTO
        {
            ClassId = c.Id,
            ClassName = c.Name
        }).ToList();
    }
}