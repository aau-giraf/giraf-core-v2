using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

namespace giraf_core_v2.DTOs.Images;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class PostGeneratedImageRequestDTO : PostImageRequestDTO
{
    [Required]
    public string? TicketId { get; set; }
}