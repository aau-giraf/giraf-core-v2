namespace giraf_core_v2.Models;

public class Guardianship
{
    public required int CitizenId { get; init; }
    public required Citizen Citizen { get; init; }

    public required int GuardianId { get; init; }
    public required NonCitizen Guardian { get; init; }
}