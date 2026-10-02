using System.ComponentModel.DataAnnotations;

public class Citizen
{
    [Key]
    public int UserId;
    public required User User;

    public int GuardianId { get; set; }
    public required User Guardian {get; set; }

    public int ClassId { get; set; }
    public required Class Class { get; set; }

}