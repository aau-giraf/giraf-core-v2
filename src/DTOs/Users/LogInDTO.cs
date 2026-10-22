using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class LogInDTO
{
    [Length(3, 255)]
    public string? Username { get; set; }

    [Required]
    [Length(8, 255)]
    public required string Password { get; set; }
}