public class UserService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<User?> GetCurrentUserAsync(int Id)
    {
        return await _db.Users.FindAsync(Id);
    }

    public async Task<bool> DeleteCurrentUserAsync(int Id)
    {
        // return true on successful update
        if (await _db.Users.FindAsync(Id) is User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}