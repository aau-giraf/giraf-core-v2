namespace giraf_core_v2.Data.Seeding;

public class UserOrganizationSeeding : ISeeding
{
    public int SeedingPosition { get; init; } = 5;
    public void Seed(AppDbContext context)
    {
        if (context.Set<UserOrganization>().Any()) return;

        Organization egebakken = context.Set<Organization>().First(org => org.Name == "Egebakken");
        Organization birkehoejen = context.Set<Organization>().First(org => org.Name == "Birkehøjen");

        User[] users = context.Set<User>().ToArray();

        UserOrganization[] userOrganizations = [
            new() { UserId = users[0].Id, User = users[0], OrganizationId = egebakken.Id,  Organization = egebakken  },
            new() { UserId = users[1].Id, User = users[1], OrganizationId = egebakken.Id,  Organization = egebakken  },
            new() { UserId = users[2].Id, User = users[2], OrganizationId = egebakken.Id,  Organization = egebakken  },
            new() { UserId = users[3].Id, User = users[3], OrganizationId = birkehoejen.Id, Organization = birkehoejen },
            new() { UserId = users[4].Id, User = users[4], OrganizationId = birkehoejen.Id, Organization = birkehoejen },
            new() { UserId = users[5].Id, User = users[5], OrganizationId = birkehoejen.Id, Organization = birkehoejen }
        ];

        context.Set<UserOrganization>().AddRange(userOrganizations);
        context.SaveChanges();
    }
}
