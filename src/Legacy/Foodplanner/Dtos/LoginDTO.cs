// Copied from foodplanner-api: FoodplannerModels/Account/User/LoginDTO.cs (only the namespace changed)
using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// Login DTO class
// Used in 'user controller' to verify login
public class LoginDTO
{
    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
    [StringLength(100, ErrorMessage = "Email er for lang")]
    public required string Email { get; set; }

    [Required(ErrorMessage = "Password er påkrævet")]
    [StringLength(100)]
    public required string Password { get; set; }
}