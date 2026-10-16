using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Models;

public class Organization
{
    [Key]
    public int Id { get; init; }
    public required string Name { get; set; }

    //Navigational property: the classes belonging to the organization
    public ICollection<Class> Classes { get; } = [];

    //Navigational property: user roles related to the organization
    public ICollection<UserRole> UserRoles { get; } = [];

    //Navigational property: The images belonging to this organization
    public ICollection<Image> Images { get; } = [];
}
