using giraf_core_v2.Auth;
using giraf_core_v2.Utils;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class UserEndpoints
{
    public static void MapUserEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users");

        group.MapGet("/me", GetCurrentUser).RequireAuthorization();
        group.MapDelete("/me", DeleteCurrentUser).RequireAuthorization();
        group.MapPatch("/me", UpdateCurrentUser).RequireAuthorization();
        group.MapPut("/me/password", UpdateUserPassword).RequireAuthorization();
        group.MapPost("/login", Login).RequireRateLimiting("rateLimitByIp");
    }

    private static async Task<Results<Ok<User>, NotFound>> GetCurrentUser(UserService userService, TokenProvider tokenProvider)
    {
        var user = await userService.GetCurrentUserAsync(1);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> DeleteCurrentUser(UserService userService)
    {
        // TODO: Get ID when authentication is enabled.
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

    private static async Task<Results<Ok<LoginResultDTO>, UnauthorizedHttpResult>> Login(
        UserService userService,
        HttpContext httpContext,
        [FromBody] LoginRequestDTO loginRequestDTO
    )
    {
        var loginResponse = await userService.Login(loginRequestDTO);

        if (loginResponse is null)
        {
            return TypedResults.Unauthorized();
        }

        // In addition to the DTO this endpoint returns, httpContext allows a cookie containing the JWT to be set.
        httpContext.Response.Cookies.Append("jwt", loginResponse.Jwt, new CookieOptions
        {
            HttpOnly = true, // Ensure the cookie can not be accessed by client side scripts.
            Secure = true,
            SameSite = SameSiteMode.Lax, // Send the cookie in requests under the same domain as the requester.
            Expires = DateTimeOffset.UtcNow.AddMilliseconds(
                double.Parse(Env.Get("JWT_MAX_AGE_MS")
                )),
            Path = "/" // The cookie can be used for all request paths.
        });

        return TypedResults.Ok(new LoginResultDTO
        {
            FirstName = loginResponse.FirstName,
            LastName = loginResponse.LastName,
            Username = loginResponse.Username,
        });
    }
}