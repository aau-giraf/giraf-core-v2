using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;


public static class LegacyChildrenEndpoints
{
    public static void MapLegacyChildrenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/children");
        //group.MapGet("/GetAllChldrenClasses", ); //Kræver teacher authorization
        //group.MapGet("/GetChildrenByParentToken", );
        //group.MapGet("/GetChildFromChildId/{id}", ); //Kræver teacher authorization
        //group.MapGet("/GetParentsByChildId/{childId}", );
        //group.MapGet("/GetTeachersByChildId/{childId}", ); //Kræver teacher authorization
        //group.MapGet("/GetChildrenByTeacherId/{teacherId}", ); //Kræver teacher authorization

        //group.MapPost("/AddParentToChild/{childId}/parents/{userId}",);
        //group.MapPost("/AddTeacherToChild/{childId}/teachers/{userId}",); //Kræver teacher authorization

        //group.MapDelete("/RemoveParentFromChild/{childId}/parents/{userId}",);
        //group.MapDelete("/RemoveTeacherFromChild/{childId}/teachers/{userId}",); //Kræver teacher authorization




    }



}