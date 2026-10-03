using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
public class Organization
{
    [NotNull]
    [Key]
    public required Guid Id { get; set; }

    [MaxLength(200)]
    public required string? Name { get; set; }

}