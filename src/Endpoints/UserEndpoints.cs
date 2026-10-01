using Microsoft.AspNetCore.Http.HttpResults;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/me", GetCurrentUser);
    }

    private static async Task<IResult> GetCurrentUser(UserService userService)
    {
        // TODO: Get ID from somewhere. (Might need authentication enabled first.)
        var user = await userService.GetCurrentUserAsync(1);
        return user == null ? Results.NotFound() : Results.Ok(user);
    }
}