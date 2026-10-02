using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Models;

//Configure a composite key of the two Ids in order
[PrimaryKey(nameof(UserId), nameof(OrganizationId))]
public class UserOrganization
{
    //Foreign key to User
    public int UserId {get; init; }
    public required User User { get; init; }

    //Foreign key to Organization
    public required int OrganizationId {get; init;}
    public required Organization Organization { get; init; }
}