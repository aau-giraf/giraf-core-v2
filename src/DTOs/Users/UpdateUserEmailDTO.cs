using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class UpdateEmailDTO
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string NewEmail { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string OldEmail { get; set; }

    [Required]
    [Length(8, 255)]
    public required string Password { get; set; }
}