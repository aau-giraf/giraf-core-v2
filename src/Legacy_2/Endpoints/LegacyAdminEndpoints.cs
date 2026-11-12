using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;


public static class LegacyAdminEndpoints
{
    public static void MapLegacyAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/admin");

        //Alt kræver admin authorization

        //group.MapGet("/GetAllUsers", ); //mangler fra core
        group.MapGet("/Get/{id}", GetNonCitizen); //MANGLER AUTHORIZATION OG USER ROLE
        //group.MapGet("/GetAllChildren", ); //mangler fra core

        //group.MapPut("/UpdateUser/{id}",);
        //group.MapPut("/UpdateChild/{id}",);

        //group.MapDelete("/Delete/{id}",);
    }


    private static async Task<Results<Ok<LegacyUserDTO>, NotFound>> GetNonCitizen(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        var foodUser = new LegacyUserDTO { Id = nonCitizen.Id, FirstName = nonCitizen.FirstName, LastName = nonCitizen.LastName, Email = nonCitizen.Email, Role = LegacyUserRole.Admin, RoleApproved = true };
        return foodUser == null ? TypedResults.NotFound() : TypedResults.Ok(foodUser);
    }

}