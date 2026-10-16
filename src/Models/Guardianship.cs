using Microsoft.EntityFrameworkCore;
namespace giraf_core_v2.Models;

//Configure a composite key in order.
[PrimaryKey(nameof(CitizenId), nameof(GuardianId))]
public class Guardianship
{
    public required int CitizenId { get; init; }
    public required Citizen Citizen { get; init; }

    public required int GuardianId { get; init; }
    public required NonCitizen Guardian { get; init; }
}