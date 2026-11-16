using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.DTOs.Images;

public class PostImagePromptDTO
{
    [Required]
    [Length(3, 250)]
    public required string Prompt { get; set; }
    
    public int? CitizenId { get; set; }
    public int? ClassId { get; set; }
    public int? OrganizationId { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        int ownerCount = 0;
        if (CitizenId != null) ownerCount++;
        if (ClassId != null) ownerCount++;
        if (OrganizationId != null) ownerCount++;

        if (ownerCount != 1)
        {
            yield return new ValidationResult(
                "You must pass exactly 1 owner (CitizenId, ClassId or OrganizationId)",
                new[] { nameof(CitizenId), nameof(ClassId), nameof(OrganizationId) }
            );
        }
    }
}