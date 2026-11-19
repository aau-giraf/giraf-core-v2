using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.UserOrganization;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class CreateUserOrganizationResponseDTO
{
    [Required]
    public int UserId { get; init; }

    [Required]
    public required int OrganizationId { get; init; }

}