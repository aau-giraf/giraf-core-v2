using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Models;

public class Organization
{
    [Key]
    public int Id { get; init; }
    public required string Name {get; set;}

    //Navigational property for users belonging to an organization
    public ICollection<UserOrganization> Users { get; } = [];
}
