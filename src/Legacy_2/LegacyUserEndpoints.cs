using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2;

public static class LegacyUserEndpoints
{
    public static void MapLegacyUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/users");

        group.MapDelete("/delete", DeleteCurrentUser);
        group.MapGet("/getLoggedIn",GetCurrentUser);


    }

    private static async Task<Results<Ok<User>, NotFound>> GetCurrentUser(UserService userService)
    {
        // TODO: Get ID from authentication.

        var user = await userService.GetCurrentUserAsync(1);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
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