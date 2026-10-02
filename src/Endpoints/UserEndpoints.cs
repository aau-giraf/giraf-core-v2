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
    }

    private static async Task<IResult> GetCurrentUser(UserService userService)
    {
        // TODO: Get ID from somewhere. (Might need authentication enabled first.)
        var user = await userService.GetCurrentUserAsync(1);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }

    private static async Task<IResult> DeleteCurrentUser(UserService userService)
    {
        // TODO: Get ID when authentication is enabled.
        var result = await userService.DeleteCurrentUserAsync(1);
        return result ? Results.NoContent() : Results.NotFound();
    }

    private static async Task<IResult> UpdateCurrentUser(
        UserService userService, [FromBody] UpdateUserDTO? inputUser)
    {
        if (inputUser is null)
        {
            return Results.BadRequest();
        }
        var user = await userService.UpdateCurrentUserAsync(1, inputUser);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }
}