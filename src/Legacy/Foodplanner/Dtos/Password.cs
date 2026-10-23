// Copied from foodplanner-api: FoodplannerModels/Account/User/Password.cs (only the namespace changed)
using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// Entity for password
// Used in user contorller to update password
public class Password
{
    [Required]
    public required string password { get; set; }
}