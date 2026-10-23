using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// PUT api/Users/UpdatePinCode (foodplanner-api UsersController.UpdatePinCode)
// Sets the logged-in user's pin code. Stored hashed, the same way as passwords.
//
// Old behaviour (roles Child and Parent):
//   201, no body
//   400 + ErrorResponse { Message = ["Pinkode skal være 4 cifre"] }   if the pin isn't exactly 4 characters
//
// v2 today: users have no pin code. Needs a PinCode field on the user model (and a migration) first
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record UpdatePinCodeRequest([FromBody] Pincode Body);

public sealed class UpdatePinCodeAdapter : ILegacyAdapter<UpdatePinCodeRequest, NoBody>
{
    public Task<LegacyResult<NoBody>> HandleAsync(UpdatePinCodeRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<NoBody>>(LegacyResult.Status(501));
    }
}