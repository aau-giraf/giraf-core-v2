using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// POST api/Users/LoginChild (foodplanner-api UsersController.LoginChild)
// Logs a child in. The old backend checked ONLY the email: no password, and Code was ignored. No login needed.
//
// Old behaviour:
//   200 + UserCredsDTO (JWT, Role, RoleApproved)
//   400 + ModelState errors, e.g. { "Email": ["Email er ikke gyldig"] }   if the body breaks the DTO's attributes
//   400 + ErrorResponse { Message = ["Email or password is wrong"] }      if no user has that email
//
// v2 today: no login yet, and v2 citizens have no email. Depends on the open "Login and tokens" decision
// in src/Legacy/README.md
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record LoginChildRequest([FromBody] LoginChild Body);

public sealed class LoginChildAdapter : ILegacyAdapter<LoginChildRequest, UserCredsDTO>
{
    public Task<LegacyResult<UserCredsDTO>> HandleAsync(LoginChildRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<UserCredsDTO>>(LegacyResult.Status(501));
    }
}