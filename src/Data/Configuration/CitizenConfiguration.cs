namespace giraf_core_v2.Data.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CitizenConfiguration : IEntityTypeConfiguration<Citizen> {


    // Explicitly configures database mapping of Citizen model - conventions were insufficient to determine mapping (shadow properties were created)
    public void Configure(EntityTypeBuilder<Citizen> builder) {
        builder.HasOne(citizen => citizen.User)
            .WithOne(user => user.Citizen)
            .HasForeignKey<Citizen>(citizen => citizen.UserId);

        builder.HasOne(citizen => citizen.Guardian)
            .WithMany()
            .HasForeignKey(citizen => citizen.GuardianId);
    }

    public static void Seed(AppDbContext context) {
        
        if (!context.Set<Citizen>().Any()) {

            User citizen1 = context.Set<User>().First(user => user.Username == "hans123");
            User citizen2 = context.Set<User>().First(user => user.Username == "markvad");
            User citizen3 = context.Set<User>().First(user => user.Username == "larslars");

            User guardian1 = context.Set<User>().First(user => user.Username == "lone123");
            User guardian2 = context.Set<User>().First(user => user.Username == "kurthansen");
            User guardian3 = context.Set<User>().First(user => user.Username == "brugernavn");

            Class class1 = context.Set<Class>().First(class_ => class_.Name == "3.Y");
            Class class2 = context.Set<Class>().First(class_ => class_.Name == "4.B");

            var citizens = new[] { 
                new Citizen{ User = citizen1, GuardianId = guardian1.Id, Guardian = guardian1, ClassId = class1.Id, Class = class1 },
                new Citizen{ User = citizen2, GuardianId = guardian1.Id, Guardian = guardian2, ClassId = class1.Id, Class = class1 },
                new Citizen{ User = citizen3, GuardianId = guardian1.Id, Guardian = guardian3, ClassId = class2.Id, Class = class2 }
            };

            context.Set<Citizen>().AddRange(citizens);
            context.SaveChanges();
        }
    }

}