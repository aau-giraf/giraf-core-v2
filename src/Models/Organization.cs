using System.ComponentModel.DataAnnotations;

public class Organization
{
    [Key]
    public int Id { get; init; }
    public required string Name {get; set;}

    public ICollection<UserOrganization> Users { get; } = [];
}