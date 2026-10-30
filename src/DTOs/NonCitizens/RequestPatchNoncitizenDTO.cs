using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class RequestPatchNonCitizenDTO
{
    [Length(3, 255)]
    public required string FirstName { get; set; }

    [Length(3, 255)]
    public required string LastName { get; set; }

    [EmailAddress]
    public required string Email { get; set; }

    [Length(3, 255)]
    public required string Username { get; set; }

    [Length(3, 255)]
    public required string NewPassword { get; set; }

    [Length(3, 255)]
    public required string OldPassword { get; set; }

}