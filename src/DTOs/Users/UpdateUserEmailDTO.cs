using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public partial class UpdateUserEmailDTO : IValidatableObject
{
    [Required]
    [MaxLength(255)]
    [ValidEmail]
    public required string NewEmail { get; set; }

    [Required]
    [MaxLength(255)]
    [ValidEmail]
    public required string OldEmail { get; set; }

    [Required]
    [Length(8, 255)]
    public required string Password { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.Equals(OldEmail, NewEmail, StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult(
                "The new email must be different from the old one",
                [nameof(NewEmail)]);
    }
}