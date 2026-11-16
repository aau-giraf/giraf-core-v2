using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class UpdatePasswordDTO
{
    [Required]
    [Length(8, 255)]
    public required string NewPassword { get; set; }

    [Required]
    [Length(8, 255)]
    public required string OldPassword { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (NewPassword == OldPassword)
            yield return new ValidationResult("The new password must be different from the old one", [nameof(NewPassword)]);
    }
}