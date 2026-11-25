using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;


public static class LegacyAdminEndpoints
{
    public static void MapLegacyAdminEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/api/admin");

        //Alt kræver admin authorization

        //group.MapGet("/GetAllUsers", ); //mangler fra core
        group.MapGet("/Get/{nonCitizenId}", GetNonCitizen); //MANGLER AUTHORIZATION OG USER ROLE
        //group.MapGet("/GetAllChildren", ); //mangler fra core

        group.MapPut("/UpdateUser/{userId}", UpdateCurrentUser);
        //group.MapPut("/UpdateChild/{id}",);

        //group.MapDelete("/Delete/{id}",);
    }


    private static async Task<Results<Ok<LegacyUserDTO>, NotFound>> GetNonCitizen(NonCitizenService nonCitizenService, int nonCitizenId)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        var roles = await nonCitizenService.GetUserRolesAsync(nonCitizenId);
        if (roles == null || nonCitizen == null)
        {
            return TypedResults.NotFound();
        }
        LegacyUserRole combinedRole = 0; //no role
        foreach (string role in roles.RoleOrganizationTuples)
        {
            string roleString = role.Split('|', 2)[0];
            //Tries to check if string can be parsed to enum. Is not case sensitive. Output enum in variable
            if (Enum.TryParse<LegacyUserRole>(roleString, ignoreCase: true, out LegacyUserRole parsedRole))
            {
                combinedRole |= parsedRole; //Combines roles with OR ex. Admin | Parent. 
            }

        }
        var foodUser = new LegacyUserDTO { Id = nonCitizen.Id, FirstName = nonCitizen.FirstName, LastName = nonCitizen.LastName, Email = nonCitizen.Email, Role = combinedRole, RoleApproved = true };
        return foodUser == null ? TypedResults.NotFound() : TypedResults.Ok(foodUser);
    }

    private static async Task<Results<BadRequest, NotFound, NoContent>> UpdateCurrentUser(
    NonCitizenService nonCitizenService, [FromBody] LegacyUpdateUserDTO? inputUser, int userId)
    {
        // TODO: get ID from authentication


        if (inputUser is null)
        {
            return TypedResults.BadRequest();
        }
        var coreBody = new RequestPatchNonCitizenDTO { FirstName = inputUser.FirstName, LastName = inputUser.LastName, Email = inputUser.Email };
        var coreUser = await nonCitizenService.PatchNonCitizenAsync(userId, coreBody);


        return coreUser.Item1 switch
        {
            HttpReponseType.NotFound => TypedResults.NotFound(),
            HttpReponseType.Ok => TypedResults.NoContent(),
            _ => TypedResults.BadRequest()
        };
    }

}