namespace giraf_core_v2.Legacy_2.DTOs.LegacyChildrenDTO;
using System.ComponentModel.DataAnnotations;

public class LegacyChildrenDTO
{
    public int ChildId { get; set; }

    [Required(ErrorMessage = "Fornavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Fornavn er for langt")]
    public required string FirstName { get; set; }

    [Required(ErrorMessage = "Efternavn er påkrævet")]
    [StringLength(100, ErrorMessage = "Efternavn er for langt")]
    public required string LastName { get; set; }

    public int ParentId { get; set; }

    public int? ClassId { get; set; }
}