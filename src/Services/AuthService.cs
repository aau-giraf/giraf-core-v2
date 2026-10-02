public class AuthService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<User?> RegisterUser(RegisterUserDTO registerUser)
    {
        var user = new User
        {
            FirstName = registerUser.FirstName,
            LastName = registerUser.LastName,
            Email = registerUser.Email,
            Username = registerUser.Username,
            Password = BCrypt.Net.BCrypt.HashPassword(registerUser.Password, 12),
            Role = registerUser.Role
        };

        _db.Users.Add(user);

        await _db.SaveChangesAsync();

        return user;
    }
}