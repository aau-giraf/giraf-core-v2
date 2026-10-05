using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

public class Citizen
{
    [Key]
    public int UserId { get; set; }
    public User User { get; set; } = null!;

    public int GuardianId { get; set; }
    public User Guardian {get; set; } = null!;

    public int ClassId { get; set; }
    public Class Class { get; set; } = null!;
}

// Explicitly configures database mapping of Citizen model - conventions were insufficient to determine mapping (shadow properties were created)
public sealed class CitizenConfiguration : IEntityTypeConfiguration<Citizen>
{
    public void Configure(EntityTypeBuilder<Citizen> builder)
    {
        builder.HasOne(citizen => citizen.User)
            .WithOne(user => user.Citizen)
            .HasForeignKey<Citizen>(citizen => citizen.UserId);

        builder.HasOne(citizen => citizen.Guardian)
            .WithMany()
            .HasForeignKey(citizen => citizen.GuardianId);
    }
}