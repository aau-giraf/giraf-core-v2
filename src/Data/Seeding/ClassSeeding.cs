namespace giraf_core_v2.Data.Seeding;

public class ClassSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 2;  
    public void Seed(AppDbContext context) {        
        if (!context.Set<Class>().Any()) {

            Organization egeBakken = context.Set<Organization>().First(org => org.Name == "Egebakken"); // put navn i variabel / fetch dynamisk 
            Organization birkehøjen = context.Set<Organization>().First(org => org.Name == "Birkehøjen");

            var classes = new[] { 
                new Class{ Name = "3.Y", OrganizationId = egeBakken!.Id, Organization = egeBakken }, 
                new Class{ Name = "4.B", OrganizationId = egeBakken.Id, Organization = egeBakken }, 
                new Class{ Name = "1.A", OrganizationId= birkehøjen!.Id, Organization = birkehøjen } 
            };

            context.Set<Class>().AddRange(classes);
            context.SaveChanges();
        }
    }

}