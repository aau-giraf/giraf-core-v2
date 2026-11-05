using giraf_core_v2.Auth;

using Microsoft.EntityFrameworkCore;

public class UserService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<User?> GetCurrentUserAsync(int id)
    {
        return await _db.Users.FindAsync(id);
    }

    public async Task<bool> DeleteCurrentUserAsync(int id)
    {
        // Hvem må slette en bruger? Må citizens slette deres egen?
        // Return true on successful delete.
        if (await _db.Users.FindAsync(id) is User user)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<User?> UpdateCurrentUserAsync(int id, UpdateUserDTO inputUser)
    {
        // TODO: Hvem må kalde endpointet? Og husk citizens ikke har emails.
        var user = await _db.Users.FindAsync(id);

        // Return null if user does not exist.
        if (user is null)
        {
            return null;
        }

        // Update supplied fields.
        if (inputUser.FirstName is not null && inputUser.FirstName != user.FirstName)
        {
            user.FirstName = inputUser.FirstName;
        }

        if (inputUser.LastName is not null && inputUser.LastName != user.LastName)
        {
            user.LastName = inputUser.LastName;
        }

        if (inputUser.Username is not null && inputUser.Username != user.Username)
        {
            if (await _db.Users.AnyAsync(u => u.Username == inputUser.Username))
            {
                // hvad skal kalderen få af svar hvis brugernavn er taget? samme med email.
            }
            else
            {
                user.Username = inputUser.Username;
            }
        }

        // Save changes.
        await _db.SaveChangesAsync();

        return user;
    }

    public async Task<int> UpdateUserPasswordAsync(int id, UpdatePasswordDTO updatePassword)
    {
        var user = await _db.Users.FindAsync(id);

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

    public async Task<LoginResponseDTO?> Login(LoginRequestDTO loginRequestDTO)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Username.Equals(loginRequestDTO.Username.ToLower()));

        string roles = "";

        if (user is NonCitizen)
        {
            List<UserRole> userRoles = await _db.UserRoles.Where(r => r.NonCitizenId == user.Id).ToListAsync();

            foreach (UserRole role in userRoles)
            {
                roles += role.Role.ToString() + " ";
            }
        }
        else if (user is not Citizen)
        {
            throw new Exception("User is neither of type Citizen nor NonCitizen");
        }

        // If user doesn't exist or password is incorrect, return null.
        if (user is null || !BCrypt.Net.BCrypt.Verify(loginRequestDTO.Password, user.Password))
        {
            return null;
        }

        TokenProvider tokenProvider = new();

        return new LoginResponseDTO
        {
            FirstName = user.FirstName,
            LastName = user.LastName,
            Username = user.Username,
            Jwt = tokenProvider.Create(user, roles.Trim())
        };
    }
}