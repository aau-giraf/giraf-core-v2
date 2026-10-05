using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticAssets;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/auth");

        group.MapPost("/register", RegisterUser);
    }

    private static async Task<Results<Ok, BadRequest>> RegisterUser(AuthService authService, [FromBody] RegisterUserDTO registerUser)
    {
        if (registerUser is null)
        {
            return TypedResults.BadRequest();
        }

        await authService.RegisterUser(registerUser);

        return TypedResults.Ok();
    }
}