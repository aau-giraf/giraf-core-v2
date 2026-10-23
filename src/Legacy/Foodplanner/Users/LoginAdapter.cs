using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// POST api/Users/Login (foodplanner-api UsersController.Login)
// Logs a parent, teacher or admin in with email and password. No login needed.
//
// Old behaviour:
//   200 + UserCredsDTO (JWT, Role, RoleApproved)
//   400 + ModelState errors, e.g. { "Email": ["Email er ikke gyldig"] }   if the body breaks the DTO's attributes
//   400 + ErrorResponse { Message = ["Email or password is wrong"] }      if the email or password is wrong
//
// v2 today: no login yet (the auth branch adds UserService.Login, by username). Depends on the open
// "Login and tokens" decision in src/Legacy/README.md
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record LoginRequest([FromBody] LoginDTO Body);

public sealed class LoginAdapter : ILegacyAdapter<LoginRequest, UserCredsDTO>
{
    public Task<LegacyResult<UserCredsDTO>> HandleAsync(LoginRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<UserCredsDTO>>(LegacyResult.Status(501));
    }
}