using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.Classes;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class GetClassInOrganizationResponseDTO
{
    [Required]
    public required int ClassId { get; set; }

    [Required]
    [Length(3, 255)]
    public required string Name { get; set; }

    [Required]
    public required int OrganizationId { get; init; }

}