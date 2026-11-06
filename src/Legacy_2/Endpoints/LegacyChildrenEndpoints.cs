using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;


public static class LegacyChildrenEndpoints
{
    public static void MapLegacyChildrenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/children");
        //group.MapGet("/GetAllChldrenClasses", );
        //group.MapGet("/GetChildrenByParentToken", );
        //group.MapGet("/GetChildFromChildId/{id}", );
        //group.MapGet("/GetParentsByChildId/{childId}", );
        //group.MapGet("/GetTeachersByChildId/{childId}", );
        //group.MapGet("/GetChildrenByTeacherId/{teacherId}", );

        //group.MapPost("/AddParentToChild/{childId}/parents/{userId}",);
        //group.MapPost("/AddTeacherToChild/{childId}/teachers/{userId}",);

        //group.MapDelete("/RemoveParentFromChild/{childId}/parents/{userId}",);
        //group.MapDelete("/RemoveTeacherFromChild/{childId}/teachers/{userId}",);

        


    }

    

}