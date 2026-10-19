using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Metadata for an image file. The file itself is stored in object storage (MinIO), referenced by StorageKey //MinIO is a bad idea no longer maintained
public class Image
{
    [Key]
    public int Id { get; init; }
    public required string Name { get; set; }
    //Optional foreign key to the Citizen the image belongs to
    public int? CitizenId { get; set; }
    public int? ClassId { get; set; }
    public Citizen? Citizen { get; set; }
    public required string Path { get; set; }

    //Foreign key to the Organization the image is scoped to. If connected to a citizen it should be found by though the citizen and never be NULL
    public int OrganizationId { get; set; }
    public required Organization Organization { get; set; }

    public DateTime CreatedAt { get; init; }
}

// Explicitly configures database mapping of Image model - constraints, defaults and delete behavior cannot be expressed by conventions
public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(image => image.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.HasOne(image => image.Citizen)
            .WithMany()
            .HasForeignKey(image => image.CitizenId);

        builder.HasOne(image => image.Organization)
            .WithMany()
            .HasForeignKey(image => image.OrganizationId);
    }
}
