using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;
#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class PostImageDTO : IValidatableObject
{
    [Required]
    [Length(3, 255)]
    public required string Name { get; set; }

    [Required]
    [Length(3, 255)]
    public required IFormFile File { get; set; }

    public int? CitizenId { get; set; }
    public int? ClassId { get; set; }
    public int? OrganizationId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Returns error if there is no way to identify who the Returns error if there is no way to identify who the picture belogs to
        if (CitizenId == null && ClassId == null && OrganizationId == null)
        {
            yield return new ValidationResult(
                "Enten CitizenId, ClassId eller Organization skal være udfyldt.",//maybe in english
                new[] { nameof(CitizenId), nameof(ClassId), nameof(Organization) }
            );
        }

        HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "jpeg", "jpg", "png", "heic", "webp"
        };
        string ext = Path.GetExtension(File.FileName);

        // Returns error if DTO isn't the right type
        if (!string.IsNullOrEmpty(ext) && allowedExtensions.Contains(ext))
        {
            yield return new ValidationResult(
                "Filen, som er blevet uploadet har ikke den rigtige filtype", //maybe in english
                new[] { nameof(ext) }
            );
        }

    }
}