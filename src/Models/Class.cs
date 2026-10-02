public class Class
{
    public int Id { get; init; }
    public required string Name { get; set; }

    public int OrganizationId { get; init; }
    public required Organization Organization { get; init; }

    //Navigational property for citizens belonging to a class
    public ICollection<Citizen> Citizens { get; } = [];
}