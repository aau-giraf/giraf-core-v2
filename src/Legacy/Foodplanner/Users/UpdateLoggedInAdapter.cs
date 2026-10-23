using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// PUT api/Users/UpdateLoggedIn (foodplanner-api UsersController.UpdateLoggedIn)
// Updates the logged-in user's first name, last name and email.
//
// Old behaviour (any role):
//   201, no body   updated
//   404, no body   the user doesn't exist

// Everything the endpoint receives. The old method took [FromBody] UserUpdateLoggedInDTO,
// so the record has one [FromBody] property of that type. The token is not needed here: see context.CallerId
public sealed record UpdateLoggedInRequest([FromBody] UserUpdateLoggedInDTO Body);

// TResponse is NoBody, because the old endpoint returned Created() without a body
public sealed class UpdateLoggedInAdapter(UserService users) : ILegacyAdapter<UpdateLoggedInRequest, NoBody>
{
    public async Task<LegacyResult<NoBody>> HandleAsync(
        UpdateLoggedInRequest request, LegacyCallContext context, CancellationToken ct)
    {
        // 1. Who is calling. The old endpoint had [Authorize], so no logged-in user means 401
        if (context.CallerId is not int callerId)
        {
            return LegacyResult.Unauthorized();
        }

        // 2. Translate the old request into what the v2 service expects.
        // The email is NOT saved yet: v2's UpdateCurrentUserAsync ignores Email, and citizens have no email in v2.
        // The old app still gets 201, so it believes the email changed. Save it here once the team decides how
        var update = new UpdateUserDTO
        {
            FirstName = request.Body.FirstName,
            LastName = request.Body.LastName,
        };

        // 3. Call v2
        var user = await users.UpdateCurrentUserAsync(callerId, update);

        // 4. Translate the v2 result into the old response
        return user is null ? LegacyResult.NotFound() : LegacyResult.Created();
    }
}