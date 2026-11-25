using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;
namespace giraf_core_v2.Legacy_2;


public static class LegacyUserEndpoints
{
    public static void MapLegacyUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/api/Users");

        group.MapDelete("/deleteLoggedInUser", DeleteCurrentUser);


        group.MapGet("/GetLoggedIn", GetCurrentUser);
        //group.MapGet("/GetBearerTest", );
        //group.MapGet("/EmailExists", );
        //group.MapGet("/HasPinCode", );

        //group.MapPut("/UpdateLoggedIn", UpdateCurrentUser);
        //group.MapPut("/UpdatePinCode", );
        //group.MapPut("/UpdatePassword", );

        //group.MapPost("/Create",);
        //group.MapPost("/CreateUserChildren",);
        //group.MapPost("/Login",);
        //group.MapPost("/LoginChild",);
        //group.MapPost("/ChekcPinCode",);


    }

    private static async Task<Results<Ok<LegacyUserDTO>, NotFound>> GetCurrentUser(NonCitizenService nonCitizenService)
    {
        // TODO: Get ID when authentication is enabled.
        //currently just user 1
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(1);
        var roles = await nonCitizenService.GetUserRolesAsync(1);
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

    private static async Task<Results<NoContent, NotFound, BadRequest>> DeleteCurrentUser(UserService userService)
    {
        // TODO: Get ID when authentication is enabled.
        int id = 1;
        if (id is -1)
        {
            return TypedResults.BadRequest();
        }

        bool result = await userService.DeleteCurrentUserAsync(id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}