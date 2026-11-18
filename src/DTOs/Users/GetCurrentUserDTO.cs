using System.ComponentModel.DataAnnotations;

public class GetCurrentUserDTO
{
    [Required]
    public int Id { get; init; }

    [Required]
    [MaxLength(255)]
    public required string FirstName { get; init; }

    [Required]
    [MaxLength(255)]
    public required string LastName { get; init; }

    [Required]
    [MaxLength(255)]
    public required string UserName { get; init; }

    [MaxLength(255)]
    [ValidEmail]
    public string? Email { get; set; }
    
    [Required]
    public List<int> OrganizationIds { get; set; } =  [];
}