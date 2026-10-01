using System.ComponentModel.DataAnnotations;
public class Organization
{
    public required int Id { get; set; }

    [MaxLength(200)]
    public required string? Name { get; set; }

}