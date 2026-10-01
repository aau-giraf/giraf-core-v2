public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/me", GetCurrentUser);
        group.MapDelete("/me", DeleteCurrentUser);
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
}