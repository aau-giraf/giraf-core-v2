// Copied from foodplanner-api: FoodplannerModels/Account/User/LoginChild.cs (only the namespace changed)
using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// Entity class for logging in a child
// Used in 'user' controller
public class LoginChild
{
    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
    [StringLength(100, ErrorMessage = "Email er for lang")]
    public required string Email { get; set; }
    public required string? Code { get; set; }
}