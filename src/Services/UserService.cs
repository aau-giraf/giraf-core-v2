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

    public async Task<User?> UpdateCurrentUserAsync(int Id, UpdateUserDTO inputUser)
    {
        var user = await _db.Users.FindAsync(Id);

        // Return null if user does not exist.
        if (user is null)
        {
            return null;
        }

        // Update supplied fields.
        if (inputUser.FirstName is not null)
        {
            user.FirstName = inputUser.FirstName;
        }

        if (inputUser.LastName is not null)
        {
            user.LastName = inputUser.LastName;
        }

        if (inputUser.Email is not null)
        {
            user.Email = inputUser.Email;
        }

        if (inputUser.Username is not null)
        {
            user.Username = inputUser.Username;
        }

        // Save changes.
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<int> UpdateUserPasswordAsync(int Id, UpdatePasswordDTO updatePassword)
    {
        var user = await _db.Users.FindAsync(Id);

        // Return 0 if user does not exist.
        if (user is null)
        {
            return 0;
        }

        // Verify the supplied current password against the stored hash.
        if (!BCrypt.Net.BCrypt.Verify(updatePassword.OldPassword, user.Password))
        {
            return 1;
        }

        // Update the user's password.
        user.Password = BCrypt.Net.BCrypt.HashPassword(updatePassword.NewPassword, 12);

        await _db.SaveChangesAsync();

        // Return 2 on successful password change.
        return 2;
    }
}