// Copied from foodplanner-api: FoodplannerModels/Account/User/UserUpdateLoggedInDTO.cs (only the namespace changed)
using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// DTO to update user, when already logged in
public class UserUpdateLoggedInDTO
{
    [Required(ErrorMessage = "Fornavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Fornavn er for langt")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Efternavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Efternavn er for langt")]
    public required string LastName { get; set; }

    [Required(ErrorMessage = "Email er påkrævet")]
    [EmailAddress(ErrorMessage = "Email er ikke gyldig")]
    [StringLength(100, ErrorMessage = "Email er for langt")]
    public required string Email { get; set; }
}