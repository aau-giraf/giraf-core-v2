namespace giraf_core_v2.Models;

public class NonCitizen : User
{
    public required string Email { get; set; }

    //Navigational property: The guardianships with this non-citizen as guardian. 
    public ICollection<Guardianship> Guardianships { get; } = [];

    //Navigational property: Roles of the Non-Citizen - possibly within different organizations
    public ICollection<UserRole> Roles { get; } = [];
}