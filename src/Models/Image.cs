using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Metadata for an image file. The file itself is stored in object storage (MinIO), referenced by StorageKey //MinIO is a bad idea no longer maintained
public class Image
{
    [Key]
    public int Id { get; init; }

    public string? TitleDa { get; set; }
    public string? TitleEn { get; set; }

    public required string Path { get; set; }
    public DateTime CreatedAt { get; init; }

    public required OriginApp OriginApp { get; init; }

    //Optional foreign key to the Citizen the image belongs to
    public int? CitizenId { get; set; }
    public Citizen? Citizen { get; set; }

    public int? ClassId { get; set; }
    public Class? Class { get; set; }

    //Foreign key to the Organization the image is scoped to. If connected to a citizen it should be found by though the citizen and never be NULL
    public required int OrganizationId { get; set; }
    public Organization? Organization { get; set; }

    public string? TicketId { get; set; }
}

public enum OriginApp
{
    Foodplanner,
    VTA
}

// Explicitly configures database mapping of Image model - constraints, defaults and delete behavior cannot be expressed by conventions
public sealed class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> builder)
    {
        builder.Property(image => image.CreatedAt)
            .HasDefaultValueSql("now()");

        builder.HasOne(image => image.Citizen)
            .WithMany(citizen => citizen.Images)
            .HasForeignKey(image => image.CitizenId);

        builder.HasOne(image => image.Organization)
            .WithMany(organization => organization.Images)
            .HasForeignKey(image => image.OrganizationId);

        builder.HasIndex(image => image.TicketId).IsUnique();

        //Store enum as string, rather than the backing integer. Fragility if addition or reordering, e.g. adding new value in the middle.
        builder.Property(image => image.OriginApp)
            .HasConversion<string>();
    }
}