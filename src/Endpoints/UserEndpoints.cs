using System.Security.Claims;

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
        group.MapDelete("/{id:int}", DeleteCurrentUser).RequireAuthorization();
        group.MapPatch("/me", UpdateCurrentUser).RequireAuthorization();
        group.MapPut("/me/password", UpdateUserPassword).RequireAuthorization(); // burde password change indgå i update current user?
        group.MapPut("/me/email", UpdateUserEmail).RequireAuthorization();
        group.MapPost("/login", Login).RequireRateLimiting("rateLimitByIp");
    }

    private static int GetCurrentUserId(ClaimsPrincipal currentUser)
    {
        return int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in JWT claim!"));
    }

    private static async Task<Results<Ok<User>, NotFound>> GetCurrentUser(UserService userService, TokenProvider tokenProvider, ClaimsPrincipal currentUser)
    {
        var user = await userService.GetCurrentUserAsync(GetCurrentUserId(currentUser));
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<NoContent, NotFound, ForbidHttpResult, BadRequest<string>>> DeleteCurrentUser(int id, UserService userService, ClaimsPrincipal currentUser)
    {
        var result = await userService.DeleteCurrentUserAsync(GetCurrentUserId(currentUser), id);
        return result switch
        {
            UserServiceResult.Succes => TypedResults.NoContent(),
            UserServiceResult.NotFound => TypedResults.NotFound(),
            UserServiceResult.Forbidden => TypedResults.Forbid(),
            UserServiceResult.CanNotDeleteSelf => TypedResults.BadRequest("You can not delete yourself"),
            _ => throw new InvalidOperationException($"Unexpected result: {result}")
        };
    }

    private static async Task<Results<BadRequest, NotFound, Ok<User>>> UpdateCurrentUser(
        UserService userService, ClaimsPrincipal currentUser, [FromBody] UpdateUserDTO inputUser)
    {
        var user = await userService.UpdateCurrentUserAsync(GetCurrentUserId(currentUser), inputUser);
        return user == null ? TypedResults.NotFound() : TypedResults.Ok(user);
    }

    private static async Task<Results<BadRequest, NotFound, UnauthorizedHttpResult, NoContent>> UpdateUserPassword(UserService userService, ClaimsPrincipal currentUser, [FromBody] UpdatePasswordDTO updatePassword)
    {
        var status = await userService.UpdateUserPasswordAsync(GetCurrentUserId(currentUser), updatePassword);

        return status switch
        {
            0 => TypedResults.NotFound(),
            1 => TypedResults.Unauthorized(),
            2 => TypedResults.NoContent(),
            _ => TypedResults.BadRequest(),
        };
    }

    public static async Task<Results<NoContent, Conflict<string>, BadRequest<string>>> UpdateUserEmail (UserService userService, ClaimsPrincipal currentUser, [FromBody] UpdateEmailDTO updateEmail)
    {
        var result = await userService.UpdateUserEmailAsync(GetCurrentUserId(currentUser), updateEmail);

        return result switch
        {
            UserServiceResult.NotFound => TypedResults.BadRequest("Could not find the user"),
            UserServiceResult.Succes => TypedResults.NoContent(),
            UserServiceResult.EmailTaken => TypedResults.BadRequest("Email is already taken"),
            UserServiceResult.WrongPassword => TypedResults.Conflict("Wrong Password"),
            UserServiceResult.EmailNotAllowedForCitizen => TypedResults.BadRequest("Citizens can not have a Email")
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