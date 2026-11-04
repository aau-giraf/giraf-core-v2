using System.ComponentModel.DataAnnotations;
namespace giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;

public enum LegacyUserRole
{
    Admin = 1 << 0,    // 1
    Child = 1 << 1,    // 2
    Teacher = 1 << 2,    // 4
    Parent = 1 << 3     // 8
}
// User DTO - pretty much copied from foodplanner 
public class LegacyUserDTO
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

    [Required(ErrorMessage = "Role er påkrævet")]
    public required LegacyUserRole Role { get; set; }

    public required bool RoleApproved { get; set; }

    public bool Archived { get; set; }
}


