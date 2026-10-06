namespace giraf_core_v2.Data.Configuration;

public class OrganizationConfiguration {

    public static void Seed(AppDbContext context) {
        
        if (!context.Set<Organization>().Any()) {
            var organizations = new[] { 
                new Organization{ Name = "Egebakken" }, 
                new Organization{ Name = "Birkehøjen" }, 
                new Organization{ Name = "TestSkole" } 
            };

            context.Set<Organization>().AddRange(organizations);
            context.SaveChanges();
        }
    }

}
