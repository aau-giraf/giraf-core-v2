using giraf_core_v2.Legacy.Abstractions;
using giraf_core_v2.Legacy.Foodplanner.Dtos;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// GET api/Users/GetLoggedIn (foodplanner-api UsersController.GetLoggedIn)
// Returns the logged-in user.
//
// Old behaviour (any role):
//   200 + UserDTO
//   500   if the user doesn't exist (the old repository threw)
//
// v2 today: UserService.GetCurrentUserAsync(id). Put the User -> UserDTO mapping in FoodplannerMapper,
// since other adapters will need it too. Email exists only on NonCitizen, and Role is Foodplanner's
// [Flags] enum (Dtos/UserRole.cs), not v2's UserRole class
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs

// NoInput: the endpoint takes nothing but the caller
public sealed class GetLoggedInAdapter : ILegacyAdapter<NoInput, UserDTO>
{
    public Task<LegacyResult<UserDTO>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<UserDTO>>(LegacyResult.Status(501));
    }
}