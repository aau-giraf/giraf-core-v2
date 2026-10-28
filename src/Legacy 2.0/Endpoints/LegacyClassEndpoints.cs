using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2._0.Endpoints;

public static class LegacyClassEndpoints
{
    public static void MapClassEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/class");

    }


}