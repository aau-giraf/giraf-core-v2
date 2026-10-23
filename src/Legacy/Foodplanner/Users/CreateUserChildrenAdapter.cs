using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// POST api/Users/CreateUserChildren (foodplanner-api UsersController.CreateUserChildren)
// Creates a child account and links it to the logged-in parent and to a class (ClassId).
//
// Old behaviour (the caller must be a parent):
//   201 + the new child's id (int)
//   400 + ModelState errors, e.g. { "Email": ["Email er ikke gyldig"] }               if the body breaks the DTO's attributes
//   400 + ErrorResponse { Email = ["Forælder ikke fundet eller ugyldig rolle"] }      if the caller isn't a parent (yes, under Email)
//   400 + ErrorResponse { Email = ["Email eksisterer allerede"] }                     if the email is taken
//
// v2 today: no service creates citizens yet, and v2 citizens have no email
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record CreateUserChildrenRequest([FromBody] UserCreateChildDTO Body);

public sealed class CreateUserChildrenAdapter : ILegacyAdapter<CreateUserChildrenRequest, int>
{
    public Task<LegacyResult<int>> HandleAsync(CreateUserChildrenRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<int>>(LegacyResult.Status(501));
    }
}