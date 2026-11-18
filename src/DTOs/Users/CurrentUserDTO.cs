using System.ComponentModel.DataAnnotations;

public class CurrentUserDTO
{
    [Required]
    public int Id { get; init; }

    [Required]
    [MaxLength(255)]
    public required string Firstname { get; init; }

    [Required]
    [MaxLength(255)]
    public required string Lastname { get; init; }

    [Required]
    [MaxLength(255)]
    public required string Username { get; init; }

    [MaxLength(255)]
    public string? Email { get; set; }
    
    [Required]
    public List<int> OrganizationIds { get; set; } =  [];
}