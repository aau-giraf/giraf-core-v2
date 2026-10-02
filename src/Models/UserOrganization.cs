using Microsoft.EntityFrameworkCore;

[PrimaryKey(nameof(UserId), nameof(OrganizationId))] //Congigure a composite key of the two IDs in order
public class UserOrganization
{
    public int UserId {get; init; }
    public required User User { get; init; } //make the foreigh key for UserId
    public required string OrganizationId {get; init;}
    public required Organization Organization { get; init; } //make the foreigh key for OrginizationId
}