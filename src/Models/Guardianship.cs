using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

//Composite key - Invariant: Either 'Primary' or 'Entrusted' guardian (Citizen/Guardian pairs unique within table)
[PrimaryKey(nameof(CitizenId), nameof(GuardianId))]
public class Guardianship
{
    public required int CitizenId { get; init; }
    public Citizen? Citizen { get; init; }

    public required int GuardianId { get; init; }
    public NonCitizen? Guardian { get; init; }

    public required GuardianType GuardianType { get; init; }
}

public enum GuardianType
{
    Primary,  //Parent(s)
    Entrusted //Teachers, pedagogues, ect.
}

public class GuardianshipConfiguration : IEntityTypeConfiguration<Guardianship>
{
    public void Configure(EntityTypeBuilder<Guardianship> builder)
    {
        //Store enum as string, rather than the backing integer. Fragility if addition or reordering, e.g. adding new value in the middle.
        builder.Property(guardianShip => guardianShip.GuardianType)
            .HasConversion<string>();
    }
}