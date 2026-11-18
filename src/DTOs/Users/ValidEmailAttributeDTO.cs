using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter)]
public sealed partial class ValidEmailAttribute : ValidationAttribute
{
    [GeneratedRegex(@"^[\p{L}0-9._%+-]+@[\p{L}0-9.-]+\.\p{L}{2,}$")]
    private static partial Regex EmailRegex();

    public ValidEmailAttribute() : base("Invalid email.") { }

    public static bool IsValidEmail(string email) => EmailRegex().IsMatch(email);

    public override bool IsValid(object? value)
    {
        // null/empty  gets handled by [Required], so here it is true
        if (value is null)
            return true;

        return value is string email && (email.Length == 0 || IsValidEmail(email));
    }
}