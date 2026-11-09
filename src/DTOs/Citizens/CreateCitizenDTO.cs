using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.Data;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class CreateCitizenDTO
{
    [Required]
    public int ClassId { get; set; }

    [Length(3, 255)]
    public required string FirstName { get; set; }

    [Length(3, 255)]
    public required string LastName { get; set; }

    [Length(3, 255)]
    public required string Username { get; set; }

    [Length(3, 255)]
    public required string Password { get; set; }
}