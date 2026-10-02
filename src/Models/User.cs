using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Models
{
    public class User
    {
        [Key]
        public int Id {get; init;}
        public required string FirstName {get; set;}
        public required string LastName {get; set;}
        public required string Email {get; set;}

        public required string Username {get; set;}
        public required string Password {get; set;}
        
        //Navigational property, organizations the user is linked to
        public ICollection<UserOrganization> Organizations { get; } = [];
        public Citizen? Citizen { get; init; }
    }
}