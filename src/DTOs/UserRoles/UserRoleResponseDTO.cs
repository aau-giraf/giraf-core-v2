using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class UserRoleResponseDTO
{
    [Required]
    public required int NonCitizenId { get; set; }

    [Required]
    public required RoleType Role { get; set; }

    [Required]
    public required int OrganizationId { get; set; }
}