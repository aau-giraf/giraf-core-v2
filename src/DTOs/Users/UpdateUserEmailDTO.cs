using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public partial class UpdateUserEmailDTO : IValidatableObject
{
    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string NewEmail { get; set; }

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public required string OldEmail { get; set; }

    [Required]
    [Length(8, 255)]
    public required string Password { get; set; }

    [GeneratedRegex(@"^[\p{L}0-9._%+-]+@[\p{L}0-9.-]+\.\p{L}{2,}$")]
    private static partial Regex EmailRegex();

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!EmailRegex().IsMatch(OldEmail))
            yield return new ValidationResult("Invalid email.", [nameof(OldEmail)]);

        if (!EmailRegex().IsMatch(NewEmail))
            yield return new ValidationResult("Invalid email.", [nameof(NewEmail)]);

        if (string.Equals(OldEmail, NewEmail, StringComparison.OrdinalIgnoreCase))
            yield return new ValidationResult(
                "The new email must be diffent from the old one",
                [nameof(NewEmail)]);
    }
}