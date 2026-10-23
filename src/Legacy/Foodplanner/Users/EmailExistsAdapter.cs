using giraf_core_v2.Legacy.Abstractions;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// GET api/Users/EmailExists?email=... (foodplanner-api UsersController.EmailExists)
// Checks whether any user has the given email. No login needed.
//
// Old behaviour:
//   200 + { "emailExists": true/false }
//   400 + ErrorResponse { Message = ["Email is required"] }   if email is missing or blank
//
// v2 today: no method for it yet. Only non-citizens have an email (NonCitizens.Email, unique)
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs

// Nullable, so a missing ?email= reaches the adapter (which returns the old 400) instead of being rejected by ASP.NET Core
public sealed record EmailExistsRequest([FromQuery] string? Email);

// The old endpoint returned an anonymous object, new { EmailExists = exists }. This record gives the same JSON
public sealed record EmailExistsResponse(bool EmailExists);

public sealed class EmailExistsAdapter : ILegacyAdapter<EmailExistsRequest, EmailExistsResponse>
{
    public Task<LegacyResult<EmailExistsResponse>> HandleAsync(EmailExistsRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<EmailExistsResponse>>(LegacyResult.Status(501));
    }
}