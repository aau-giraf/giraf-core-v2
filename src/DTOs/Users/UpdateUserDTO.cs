using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class UpdateUserDTO
{
    [Length(3, 255)]
    public string? FirstName { get; set; }

    [Length(3, 255)]
    public string? LastName { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [Length(3, 255)]
    public string? Username { get; set; }
}