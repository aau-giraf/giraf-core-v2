using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
public class Class
{
    [Key]
    [NotNull]
    public required Guid Id { get; set; }

    [MaxLength(200)]
    public required string? Name { get; set; }
    
    [ForeignKey("OrganizationId")]
    public required Guid OrganizationId { get; set; }

}