using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class ImageResponseDTO
{
    public required int ImageId { get; set; }

    [Length(3, 255)]
    public required string Title { get; set; }

    public required string Base64File { get; set; }
}