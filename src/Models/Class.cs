using System.ComponentModel.DataAnnotations;
public class Class
{
    public required int Id { get; set; }

    [MaxLength(200)]
    public required string? Name { get; set; }

    public required int OrganizationId { get; set; }

}