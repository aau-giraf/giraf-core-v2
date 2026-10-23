using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// DELETE api/Users/DeleteLoggedInUser (foodplanner-api UsersController.DeleteLoggedInUser)
// Deletes the logged-in user. The old code comment says "DeleteLoggedIn", but the route uses the method name.
//
// Old behaviour (any role):
//   204, no body
//   404, no body   if the user doesn't exist
//
// v2 today: UserService.DeleteCurrentUserAsync(id), which returns true if the user was deleted
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs

// NoInput: the endpoint takes nothing but the caller
public sealed class DeleteLoggedInUserAdapter : ILegacyAdapter<NoInput, NoBody>
{
    public Task<LegacyResult<NoBody>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<NoBody>>(LegacyResult.Status(501));
    }
}