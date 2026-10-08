using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

public class Citizen : IEntityTypeConfiguration<Citizen>
{
    [Key]
    public int UserId { get; set; }
    public required User User { get; set; }

    public int GuardianId { get; set; }
    public required User Guardian {get; set; }

    public int ClassId { get; set; }
    public required Class Class { get; set; }


    // Explicitly configures database mapping of Citizen model - conventions were insufficient to determine mapping (shadow properties were created)
    public void Configure(EntityTypeBuilder<Citizen> builder) {
        builder.HasOne(citizen => citizen.User)
            .WithOne(user => user.Citizen)
            .HasForeignKey<Citizen>(citizen => citizen.UserId);

        builder.HasOne(citizen => citizen.Guardian)
            .WithMany()
            .HasForeignKey(citizen => citizen.GuardianId);
    }
}



