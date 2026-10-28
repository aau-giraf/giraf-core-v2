namespace giraf_core_v2.Services;

public class NonCitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<NonCitizen?> GetNonCitizenAsync(int id)
    {
        var noncitizen = _db.NonCitizens.FindAsync(id);
        
        return await noncitizen;
    }

    public async Task<NonCitizen?> CreateNonCitizenAsync(RegisterUserDTO input)
    {
        var nonCitizen = new NonCitizen
        {
            FirstName = input.FirstName,
            LastName = input.LastName,
            Email = input.Email,
            Password = input.Password,
            Username = input.Username,
        };
        
        _db.NonCitizens.Add(nonCitizen);
        await _db.SaveChangesAsync();
        return nonCitizen;
    }
}