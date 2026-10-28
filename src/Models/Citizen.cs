namespace giraf_core_v2.Models;

public class Citizen : User
{
    public int ClassId { get; set; }
    public Class? Class { get; set; }

    //Navigational property: The guardianships involving this citizen. Find this citizen's guardians
    public ICollection<Guardianship> Guardianships { get; } = [];

    //Navigational property: The images belonging to this citizen
    public ICollection<Image> Images { get; } = [];
}