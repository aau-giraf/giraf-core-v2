using System.ComponentModel.DataAnnotations;

public class Organization
{
    [Key]
    public int Id {get; set;}
    public required string Name {get; set;}
}