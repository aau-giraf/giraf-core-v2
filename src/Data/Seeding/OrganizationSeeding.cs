namespace giraf_core_v2.Data.Seeding;

public class OrganizationSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 1;
    public void Seed(AppDbContext context) {   

        if (context.Set<Organization>().Any()) return;

        Organization[] organizations = [ 
            new() { Name = "Egebakken"  }, 
            new() { Name = "Birkehøjen" }, 
            new() { Name = "TestSkole"  } 
        ];

        context.Set<Organization>().AddRange(organizations);
        context.SaveChanges();
        
    }

}
