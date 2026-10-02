using System.Net;
using Microsoft.AspNetCore.Mvc;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/me", GetCurrentUser);
        group.MapDelete("/me", DeleteCurrentUser);
        group.MapPatch("/me", UpdateCurrentUser);
        group.MapPut("/me/password", UpdateUserPassword);
    }

    private static async Task<IResult> GetCurrentUser(UserService userService)
    {
        // TODO: Get ID from authentication.

        var user = await userService.GetCurrentUserAsync(1);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> DeleteCurrentUser(UserService userService, [FromBody] int? id)
    {
        // TODO: Get ID when authentication is enabled.
        if (id is null)
        {
            return Results.BadRequest();
        }
        var result = await userService.DeleteCurrentUserAsync(id.Value);
        return result ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> UpdateCurrentUser(
        UserService userService, [FromBody] UpdateUserDTO? inputUser)
    {
        // TODO: get ID from authentication
        if (inputUser is null)
        {
            return Results.BadRequest();
        }

        var user = await userService.UpdateCurrentUserAsync(1, inputUser);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> UpdateUserPassword(UserService userService, [FromBody] UpdatePasswordDTO? updatePassword)
    {
        if (updatePassword is null)
        {
            return Results.BadRequest();
        }
        var status = await userService.UpdateUserPasswordAsync(2, updatePassword);

        return status switch
        {
            0 => Results.NotFound(),
            1 => Results.Unauthorized(),
            2 => Results.NoContent(),
            _ => Results.BadRequest(),
        };
    }
}