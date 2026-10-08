namespace giraf_core_v2.Data.Seeding;

public class CitizenSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 4;  
    public void Seed(AppDbContext context) {        
        if (!context.Set<Citizen>().Any()) {

            User[] users = context.Set<User>().ToArray();

            Class class1 = context.Set<Class>().First(class_ => class_.Name == "3.Y");
            Class class2 = context.Set<Class>().First(class_ => class_.Name == "4.B");

            var citizens = new[] { 
                new Citizen{ User = users[0], GuardianId = users[3].Id, Guardian = users[3], ClassId = class1.Id, Class = class1 },
                new Citizen{ User = users[1], GuardianId = users[4].Id, Guardian = users[4], ClassId = class1.Id, Class = class1 },
                new Citizen{ User = users[2], GuardianId = users[5].Id, Guardian = users[5], ClassId = class2.Id, Class = class2 }
            };

            context.Set<Citizen>().AddRange(citizens);
            context.SaveChanges();
        }
    }

}