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
    private static ClassroomResponseDTO GetAllClassrooms(OrganizationService organizationService)
    {
        return new ClassroomResponseDTO{
            ClassId = 1,
            ClassName = "1.A"
            };
    }


}