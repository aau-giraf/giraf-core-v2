using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// PUT api/Users/UpdatePassword (foodplanner-api UsersController.UpdatePassword)
// Sets the logged-in user's password. The old app sends only the new password, not the old one.
//
// Old behaviour (any role):
//   201, no body
//   404, no body   if the user doesn't exist
//
// v2 today: UserService.UpdateUserPasswordAsync needs the old password, which the old app doesn't send.
// The team must decide whether v2 may change a password without it before this can be built
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record UpdatePasswordRequest([FromBody] Password Body);

public sealed class UpdatePasswordAdapter : ILegacyAdapter<UpdatePasswordRequest, NoBody>
{
    public Task<LegacyResult<NoBody>> HandleAsync(UpdatePasswordRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<NoBody>>(LegacyResult.Status(501));
    }
}