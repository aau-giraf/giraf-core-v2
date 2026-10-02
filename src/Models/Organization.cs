using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Models
{
    public class Organization
    {
        [Key]
        public int Id { get; init; }
        public required string Name {get; set;}

        public ICollection<UserOrganization> Users { get; } = [];
    }
}
