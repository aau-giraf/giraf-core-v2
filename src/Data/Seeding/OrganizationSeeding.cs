namespace giraf_core_v2.Data.Seeding;

public class OrganizationSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 1;
    public void Seed(AppDbContext context) {        
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
