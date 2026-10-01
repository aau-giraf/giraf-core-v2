using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
[PrimaryKey(nameof(UserId), nameof(OrganizationId))] //make a composite key of the two IDs
public class UserOrganization
{
    public int UserId {get; set;}
    public required User User { get; set; } //make the foreigh key for UserId
    public required string OrganizationId {get; set;}
    public required Organization Organization { get; set; } //make the foreigh key for OrginizationId
}