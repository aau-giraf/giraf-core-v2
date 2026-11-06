using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;


public static class LegacyAdminEndpoints
{
    public static void MapLegacyAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/admin");
        //group.MapGet("/GetAllUsers", );
        //group.MapGet("/Get/{id}", );
        //group.MapGet("/GetAllChildren", );

        //group.MapPut("/UpdateUser/{id}",);
        //group.MapPut("/UpdateChild/{id}",);

        //group.MapDelete("/Delete/{id}",);

        


    }

    

}