using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy_2._0.Endpoints;

public static class LegacyUserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/users");

        group.MapDelete("/delete", DeleteCurrentUser);

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

    private static async Task<Results<BadRequest, NotFound, Ok<User>>> UpdateCurrentUser(
        UserService userService, [FromBody] UpdateUserDTO? inputUser)
    {
        // TODO: get ID from authentication
        if (inputUser is null)
        {
            return TypedResults.BadRequest();
        }

        var user = await userService.UpdateCurrentUserAsync(1, inputUser);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<BadRequest, NotFound, UnauthorizedHttpResult, NoContent>> UpdateUserPassword(UserService userService, [FromBody] UpdatePasswordDTO? updatePassword)
    {
        if (updatePassword is null)
        {
            return TypedResults.BadRequest();
        }
        var status = await userService.UpdateUserPasswordAsync(2, updatePassword);

        return status switch
        {
            0 => TypedResults.NotFound(),
            1 => TypedResults.Unauthorized(),
            2 => TypedResults.NoContent(),
            _ => TypedResults.BadRequest(),
        };
    }
}