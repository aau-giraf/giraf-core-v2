using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class ResponseGetNonCitizenDTO
{
    public int Id { get; set; }

    [Required]
    [Length(3, 255)]
    public required string FirstName { get; set; }
    [Required]
    [Length(3, 255)]
    public required string LastName { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    [Length(3, 255)]
    public required string Username { get; set; }

    public ICollection<UserRole> Roles { get; set; } = [];
    public ICollection<Guardianship> Guardianships { get; set; } = [];

}