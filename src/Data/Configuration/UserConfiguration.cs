namespace giraf_core_v2.Data.Configuration;

public class UserConfiguration {

    public static void Seed(AppDbContext context) {

        if (!context.Set<User>().Any()) {
            var users = new[] {
                new User {
                    FirstName = "Hans",
                    LastName = "Phillip",
                    Email = "hans.phillip@email.com",
                    Username = "hans123",
                    Password = "1234" // skal hashes (med hvad end algoritme der ender med at blive brugt)
                },
                new User {
                    FirstName = "Mark",
                    LastName = "Vad",
                    Email = "mark_vad123@gmail.com",
                    Username = "markvad",
                    Password = "5678"
                },
                new User {
                    FirstName = "Lars",
                    LastName = "Lars",
                    Email = "lars@hotmail.com",
                    Username = "larslars",
                    Password = "123lars"
                },
                new User {
                    FirstName = "Lone",
                    LastName = "Hamm",
                    Email = "lonehamm@hotmail.com",
                    Username = "lone123",
                    Password = "lonekode"
                },
                new User {
                    FirstName = "kurt",
                    LastName = "hansen",
                    Email = "kh@mail.dk",
                    Username = "kurthansen",
                    Password = "kurtskode"
                },
                new User {
                    FirstName = "ille",
                    LastName = "soren",
                    Email = "islroen@email.dk",
                    Username = "brugernavn",
                    Password = "kodeord"
                }

            };

            context.Set<User>().AddRange(users);
            context.SaveChanges();
        }
    }
}