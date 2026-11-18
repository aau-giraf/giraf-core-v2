using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.NonCitizens;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class NonCitizenResponseDTO
{
    public int NonCitizenId { get; set; }

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

}