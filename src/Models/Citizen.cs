using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Models;

public class Citizen
{
    [Key]
    public int UserId { get; set; }
    public required User User { get; set; }

    public int GuardianId { get; set; }
    public required User Guardian {get; set; }

    public int ClassId { get; set; }
    public required Class Class { get; set; }
}