namespace giraf_core_v2.Data.Seeding;

public class UserSeeding : ISeeding {

    public int SeedingPosition { get; init; } = 3;  
    public void Seed(AppDbContext context) {

        if (context.Set<User>().Any()) return;

        User[] users = [
            new() {
                FirstName = "Hans",
                LastName = "Phillip",
                Email = "hans.phillip@email.com",
                Username = "hans123",
                Password = BCrypt.Net.BCrypt.HashPassword("1234", 12)
            },
            new() {
                FirstName = "Mark",
                LastName = "Vad",
                Email = "mark_vad123@gmail.com",
                Username = "markvad",
                Password = BCrypt.Net.BCrypt.HashPassword("5678", 12)
            },
            new() {
                FirstName = "Lars",
                LastName = "Lars",
                Email = "lars@hotmail.com",
                Username = "larslars",
                Password = BCrypt.Net.BCrypt.HashPassword("123lars", 12)
            },
            new() {
                FirstName = "Lone",
                LastName = "Hamm",
                Email = "lonehamm@hotmail.com",
                Username = "lone123",
                Password = BCrypt.Net.BCrypt.HashPassword("lonekode", 12)
            },
            new() {
                FirstName = "kurt",
                LastName = "hansen",
                Email = "kh@mail.dk",
                Username = "kurthansen",
                Password = BCrypt.Net.BCrypt.HashPassword("kurtskode", 12)
            },
            new() {
                FirstName = "ille",
                LastName = "soren",
                Email = "islroen@email.dk",
                Username = "brugernavn",
                Password = BCrypt.Net.BCrypt.HashPassword("kodeord", 12)
            }
        ];

        context.Set<User>().AddRange(users);
        context.SaveChanges();
        
    }
    
}