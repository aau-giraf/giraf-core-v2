// Copied from foodplanner-api: FoodplannerModels/Account/User/UserDTO.cs (only the namespace changed)
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace giraf_core_v2.Legacy.Foodplanner.Dtos;

// User DTO
// More detailed than user create DTO
public class UserDTO
{
    [Key]
    public int Id { get; set; }

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

    // UserRole here is Foodplanner's enum (Dtos/UserRole.cs), not v2's UserRole class
    [Required(ErrorMessage = "Role er påkrævet")]
    public required UserRole Role { get; set; }

    public required bool RoleApproved { get; set; }

    public bool Archived { get; set; }
}