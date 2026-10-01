public class UserService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<User?> GetCurrentUserAsync(int Id)
    {
        return await _db.Users.FindAsync(Id);
    }

}