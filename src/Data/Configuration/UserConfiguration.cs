namespace giraf_core_v2.Data.Configuration;

public class UserConfiguration {

    public static void Seed(AppDbContext context) {

        if (!context.Set<User>().Any()) {
            var users = new[] {
                new User {
                    Id = 1,
                    FirstName = "Hans",
                    LastName = "Phillip",
                    Email = "hans.phillip@email.com",
                    Username = "hans123",
                    Password = "1234" // skal hashes (med hvad end algoritme der ender med at blive brugt)
                },
                new User {
                    Id = 2,
                    FirstName = "Mark",
                    LastName = "Vad",
                    Email = "mark_vad123@gmail.com",
                    Username = "markvad",
                    Password = "5678" // skal hases
                },
                new User {
                    Id = 3,
                    FirstName = "Lars",
                    LastName = "Lars",
                    Email = "lars@hotmail.com",
                    Username = "larslars",
                    Password = "123lars" // skal hashes
                }
            };

            context.Set<User>().AddRange(users);
            context.SaveChanges();
        }
    }
}