using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.Images;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class PostImageRequestDTO : IValidatableObject
{
    [Required]
    [Length(3, 255)]
    public string? TitleEn { get; set; }

    [Length(3, 255)]
    public string? TitleDa { get; set; }

    [Required]
    public required IFormFile File { get; set; }
    
    public int? CitizenId { get; set; }
    public int? ClassId { get; set; }
    public int? OrganizationId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        int nullCount = 0;
        nullCount = CitizenId == null ? nullCount + 1 : nullCount;
        nullCount = ClassId == null ? nullCount + 1 : nullCount;
        nullCount = OrganizationId == null ? nullCount + 1 : nullCount;
        // Returns error if there is no way to identify who the Returns error if there is no way to identify who the picture belogs to
        if (nullCount != 2)
        {
            yield return new ValidationResult(
                "You can pass only 1 identifying paramters (CitizenId, ClassId, OrganizationId)",
                new[] { nameof(PostImageRequestDTO) }
            );
        }

        HashSet<string> allowedExtensions = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".jpeg", ".jpg", ".png", ".heic", ".webp"
        };
        string fileExtension = Path.GetExtension(File.FileName);

        // Returns error if DTO isn't the right type
        if (!string.IsNullOrEmpty(fileExtension) && !allowedExtensions.Contains(fileExtension))
        {
            yield return new ValidationResult(
                "The file extension needs to be either '.jpeg', '.jpg', '.png', '.heic' or '.webp' ",
                new[] { nameof(File) }
            );
        }
    }
}