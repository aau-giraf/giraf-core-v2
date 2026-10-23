// Copied from foodplanner-api: FoodplannerModels/Account/User/UserCredsDTO.cs (only the namespace changed)
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// DTO to get user credentials : JWT token, role, and whether role approval status
public class UserCredsDTO
{
    [Required]
    public required string JWT { get; set; }

    [Required]
    public required string Role { get; set; }

    [Required]
    public required bool RoleApproved { get; set; }
}