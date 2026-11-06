using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;
namespace giraf_core_v2.Legacy_2;


public static class LegacyUserEndpoints
{
    public static void MapLegacyUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/users");

        group.MapDelete("/deleteLoggedInUser", DeleteCurrentUser);
        

        group.MapGet("/getLoggedIn", GetCurrentUser);
        //group.MapGet("/GetBearerTest", );
        //group.MapGet("/EmailExists", );
        //group.MapGet("/HasPinCode", );

        group.MapPut("/UpdateLoggedIn", UpdateCurrentUser);
        //group.MapPut("/UpdatePinCode", );
        //group.MapPut("/UpdatePassword", );

        //group.MapPost("/Create",);
        //group.MapPost("/CreateUserChildren",);
        //group.MapPost("/Login",);
        //group.MapPost("/LoginChild",);
        //group.MapPost("/ChekcPinCode",);


    }

    private static async Task<Results<Ok<LegacyUserDTO>, NotFound>> GetCurrentUser(UserService userService)
    {
        // TODO: Get ID from authentication.
        //Currently some 'test data' because we cant get neither email or role from core user
        var user = await userService.GetCurrentUserAsync(1); //get user from core
        var foodUser = new LegacyUserDTO { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = "test@email.com", Role = LegacyUserRole.Admin, RoleApproved = true };//change into foodplanner user
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

    private static async Task<Results<BadRequest, NotFound, Ok<LegacyUserDTO>>> UpdateCurrentUser(
        UserService userService, [FromBody] LegacyUpdateUserDTO? inputUser)
    {
        // TODO: get ID from authentication


        if (inputUser is null)
        {
            return TypedResults.BadRequest();
        }

        var coreUser = new UpdateUserDTO { FirstName = inputUser.FirstName, LastName = inputUser.LastName, Email = inputUser.Email };

        var user = await userService.UpdateCurrentUserAsync(1, coreUser);
        //make return user into foodplanner user
        var foodUser = new LegacyUserDTO { Id = user.Id, FirstName = user.FirstName, LastName = user.LastName, Email = inputUser.Email, Role = LegacyUserRole.Admin, RoleApproved = true };//change into foodplanner user
        return foodUser == null ? TypedResults.NotFound() : TypedResults.Ok(foodUser);
    }

}