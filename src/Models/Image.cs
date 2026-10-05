using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Metadata for an image file. The file itself is stored in object storage (MinIO), referenced by StorageKey
public class Image
{
    [Key]
    public Guid Id { get; init; }

    //Foreign key to the User who uploaded the image
    public int UserId { get; init; }
    public required User User { get; init; }

    //Optional foreign key to the Organization the image is scoped to
    public int? OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    //Optional foreign key to the Citizen the image belongs to
    public int? CitizenId { get; set; }
    public Citizen? Citizen { get; set; }

    [MaxLength(512)]
    public required string StorageKey { get; init; }

    [MaxLength(255)]
    public required string FileName { get; set; }

    [MaxLength(100)]
    public required string ContentType { get; init; }

    public long SizeBytes { get; init; }
    public DateTime CreatedAt { get; init; }
}

// Explicitly configures database mapping of Image model - constraints, defaults and delete behavior cannot be expressed by conventions
public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.HasIndex(image => image.StorageKey)
            .IsUnique();

        builder.Property(image => image.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.ToTable(table =>
        {
            table.HasCheckConstraint("CK_Image_SizeBytes_Positive", "\"SizeBytes\" > 0");
            table.HasCheckConstraint("CK_Image_ContentType_Image", "\"ContentType\" LIKE 'image/%'");
        });

        //Restrict deletes, so owners with images cannot be removed while their files remain in object storage
        builder.HasOne(image => image.User)
            .WithMany()
            .HasForeignKey(image => image.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(image => image.Organization)
            .WithMany()
            .HasForeignKey(image => image.OrganizationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(image => image.Citizen)
            .WithMany()
            .HasForeignKey(image => image.CitizenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
