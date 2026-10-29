using System.ComponentModel.DataAnnotations;

#pragma warning disable ASP0029 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
public class UpdateCitizenDTO
{
    [Range(1, int.MaxValue)]
    public int? ClassId { get; set; }
    [Length(3, 255)]
    public string? FirstName { get; set; }
    [Length(3, 255)]
    public string? LastName { get; set; }
    [Length(3, 255)]
    public string? Username { get; set; }
}