using Microsoft.AspNetCore.Http.HttpResults;

using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;
namespace giraf_core_v2.Legacy_2;


public static class LegacyUserEndpoints
{
    public static void MapLegacyUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/users");

        group.MapDelete("/delete", DeleteCurrentUser);
        group.MapGet("/getLoggedIn", GetCurrentUser);


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
        // int id = ;
        // if (id is null)
        // {
        //     return TypedResults.BadRequest();
        // }
        var result = await userService.DeleteCurrentUserAsync(1);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}