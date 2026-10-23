using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// POST api/Users/Create (foodplanner-api UsersController.Create)
// Creates a parent or teacher account. No login needed.
//
// Old behaviour:
//   201 + the new user's id (int)
//   400 + ErrorResponse { Message = ["Children must be created using the CreateUserChildren endpoint."] }   if Role is "Child"
//   400 + ModelState errors, e.g. { "Email": ["Email er ikke gyldig"] }                                    if the body breaks the DTO's attributes
//   400 + ErrorResponse { Email = ["Email eksisterer allerede"] }                                          if the email is taken
//
// v2 today: no sign-up service yet. RegisterUserDTO exists, but nothing uses it
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs
public sealed record CreateRequest([FromBody] UserCreateDTO Body);

public sealed class CreateAdapter : ILegacyAdapter<CreateRequest, int>
{
    public Task<LegacyResult<int>> HandleAsync(CreateRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<int>>(LegacyResult.Status(501));
    }
}