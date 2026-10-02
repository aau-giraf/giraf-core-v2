using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class Citizen
{
    [Key]
    public int UserId { get; set; }
    public required User User { get; set; }

    public int GuardianId { get; set; }
    public required User Guardian {get; set; }

    public int ClassId { get; set; }
    public required Class Class { get; set; }

}

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

