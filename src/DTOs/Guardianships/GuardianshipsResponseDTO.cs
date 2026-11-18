using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.Guardianships;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class GuardianshipsResponseDTO
{
    public required int NonCitizenId { get; set; }
    public required ICollection<int> CitizenIds { get; set; } = [];
}