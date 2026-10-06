using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class ResponseGetClassInOrganizationDTO
{   
    [Required]
    public required int Id {get; set;}

    [Required]
    [Length(3, 255)]
    public required string Name {get; set;}

    [Required]
    public required int OrganizationId { get; init; }
    
}