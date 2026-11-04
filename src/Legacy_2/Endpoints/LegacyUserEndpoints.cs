using Microsoft.AspNetCore.Http.HttpResults;

namespace giraf_core_v2.Legacy_2;

public static class LegacyUserEndpoints
{
    public static void MapLegacyUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/users");

        group.MapDelete("/deleteLoggedInUser", DeleteCurrentUser);
        group.MapGet("/getLoggedIn", GetCurrentUser);


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
        int id = 1;
        if (id is -1)
        {
            return TypedResults.BadRequest();
        }

        bool result = await userService.DeleteCurrentUserAsync(id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

}