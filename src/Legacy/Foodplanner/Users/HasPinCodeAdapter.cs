using giraf_core_v2.Legacy.Abstractions;

namespace giraf_core_v2.Legacy.Foodplanner.Users;

// GET api/Users/HasPinCode (foodplanner-api UsersController.HasPinCode)
// Tells whether the logged-in user has set a pin code.
//
// Old behaviour (roles Child and Parent):
//   200 + { "hasPinCode": true/false }
//
// v2 today: users have no pin code. Needs a PinCode field on the user model (and a migration) first
//
// TODO: not implemented yet, so it answers 501. How to: src/Legacy/README.md. Finished example: UpdateLoggedInAdapter.cs

// The old endpoint returned an anonymous object, new { HasPinCode = result }. This record gives the same JSON
public sealed record HasPinCodeResponse(bool HasPinCode);

// NoInput: the endpoint takes nothing but the caller
public sealed class HasPinCodeAdapter : ILegacyAdapter<NoInput, HasPinCodeResponse>
{
    public Task<LegacyResult<HasPinCodeResponse>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<HasPinCodeResponse>>(LegacyResult.Status(501));
    }
}