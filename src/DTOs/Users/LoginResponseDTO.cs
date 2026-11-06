using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class LoginResponseDTO
{
    [Required]
    [Length(2, 255)]
    public required string FirstName { get; set; }

    [Required]
    [Length(2, 255)]
    public required string LastName { get; set; }

    [Required]
    [Length(3, 255)]
    public required string Username { get; set; }

    [Required]
    public required string Jwt { get; set; }
}