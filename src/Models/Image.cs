using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Metadata for an image file. The file itself is stored in object storage (MinIO), referenced by StorageKey
public class Image
{
    [Key]
    public int Id { get; init; }
    public required string name { get; set; }
    //Optional foreign key to the Citizen the image belongs to
    public int? CitizenId { get; set; }
    public Citizen? Citizen { get; set; }
    public required string path { get; set; }

    //Foreign key to the Organization the image is scoped to. If connected to a citizen it should be found by though the citizen and never be NULL
    public int OrganizationId { get; set; }
    public required Organization Organization { get; set; }
    [MaxLength(512)]
    public required string StorageKey { get; init; }
    [MaxLength(255)]
    public required string FileName { get; set; }
    [MaxLength(100)]
    public required string FileType { get; init; }

    public long SizeBytes { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Explicitly configures database mapping of Image model - constraints, defaults and delete behavior cannot be expressed by conventions
public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        //
        builder.HasIndex(image => image.StorageKey)
            .IsUnique();

        builder.Property(image => image.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Image_SizeBytes_Positive", "\"SizeBytes\" > 0");
        });

        builder.HasOne(image => image.Citizen)
            .WithMany()
            .HasForeignKey(image => image.CitizenId);

        builder.HasOne(image => image.Organization)
            .WithMany()
            .HasForeignKey(image => image.OrganizationId);
    }
}
