namespace giraf_core_v2.Data.Configuration;

public class OrganizationConfiguration {

    public static void Seed(AppDbContext context) {
        
        if (!context.Set<Organization>().Any()) {
            var organizations = new[] { 
                new Organization{ Id = 1, Name = "Egebakken" }, 
                new Organization{ Id = 2, Name = "Birkehøjen" }, 
                new Organization{ Id = 3, Name = "Fyrdalen" } 
            };

            context.Set<Organization>().AddRange(organizations);
            context.SaveChanges();
        }
    }

}
