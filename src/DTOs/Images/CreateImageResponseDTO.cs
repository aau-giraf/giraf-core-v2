using System.ComponentModel.DataAnnotations;

using Microsoft.Extensions.Validation;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
[ValidatableType]
public class CreateImageResponseDTO
{
    public required int ImageId { get; set; }

    [Length(3, 255)]
    public required string Title { get; set; }

    public required IResult File { get; set; }

    public int OrganizationId { get; set; }

    public int? CitizenId { get; set; }

    public int? ClassId { get; set; }

    public required string Path { get; set; }

    public DateTime CreatedAt { get; set; }
}