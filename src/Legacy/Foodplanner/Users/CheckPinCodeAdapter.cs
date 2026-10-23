using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// POST api/Users/CheckPinCode (foodplanner-api UsersController.CheckPinCode)
// Checks the logged-in user's pin code and, if it matches, returns a new token.
//
// Old behaviour (roles Child and Parent):
//   200 + UserCredsDTO (JWT, Role, RoleApproved)
//   400 + ErrorResponse { Message = ["Forkert pinkode"] }   if the pin is wrong
//   500                                                     if the user has no pin code (the old repository threw)
//
// v2 today: users have no pin code. Needs a PinCode field on the user model (and a migration) first,
// and login for the token
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record CheckPinCodeRequest([FromBody] Pincode Body);

public sealed class CheckPinCodeAdapter : ILegacyAdapter<CheckPinCodeRequest, UserCredsDTO>
{
    public Task<LegacyResult<UserCredsDTO>> HandleAsync(CheckPinCodeRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<UserCredsDTO>>(LegacyResult.Status(501));
    }
}