using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class CreateCitizenDTO
{
    [Required]
    public required int UserId { get; set; }
    [Required]
    public required int GuardianId { get; set; }
    [Required]
    public required int ClassId { get; set; }
}