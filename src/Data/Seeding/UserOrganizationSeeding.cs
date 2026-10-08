namespace giraf_core_v2.Data.Seeding;

public class UserOrganizationSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 5;  
    public void Seed(AppDbContext context) {        
        if (!context.Set<UserOrganization>().Any()) {

            Organization egeBakken = context.Set<Organization>().First(org => org.Name == "Egebakken"); // put navn i variabel / fetch dynamisk 
            Organization birkehøjen = context.Set<Organization>().First(org => org.Name == "Birkehøjen");

            User[] users = context.Set<User>().ToArray();

            var userOrganizations = new[] { 
                new UserOrganization{ UserId = users[0].Id, User = users[0], OrganizationId = egeBakken.Id, Organization = egeBakken },
                new UserOrganization{ UserId = users[1].Id, User = users[1], OrganizationId = egeBakken.Id, Organization = egeBakken },
                new UserOrganization{ UserId = users[2].Id, User = users[2], OrganizationId = egeBakken.Id, Organization = egeBakken },
                new UserOrganization{ UserId = users[3].Id, User = users[3], OrganizationId = birkehøjen.Id, Organization = birkehøjen },
                new UserOrganization{ UserId = users[4].Id, User = users[4], OrganizationId = birkehøjen.Id, Organization = birkehøjen },
                new UserOrganization{ UserId = users[5].Id, User = users[5], OrganizationId = birkehøjen.Id, Organization = birkehøjen }

            };

            context.Set<UserOrganization>().AddRange(userOrganizations);
            context.SaveChanges();
        }
    }

}
