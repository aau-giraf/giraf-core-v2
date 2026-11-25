namespace giraf_core_v2.Services;

public class CitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<GetCitizenResponseDTO?> GetCitizenAsync(int citizenId)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);

        if (citizen is null)
        {
            return null;
        }

        GetCitizenResponseDTO getCitizenDto = new GetCitizenResponseDTO
        {
            UserId = citizen.Id,
            ClassId = citizen.ClassId,
            FirstName = citizen.FirstName,
            LastName = citizen.LastName,
            Username = citizen.Username,
        };

        return getCitizenDto;
    }

    public async Task<GetCitizenResponseDTO?> CreateCitizenAsync(CreateCitizenRequestDTO createCitizen)
    {
        var citizen = new Citizen
        {
            ClassId = createCitizen.ClassId,
            FirstName = createCitizen.FirstName,
            LastName = createCitizen.LastName,
            Password = BCrypt.Net.BCrypt.HashPassword(createCitizen.Password, 12),
            Username = createCitizen.Username,
        };

        var schoolClass = await _db.Classes.FindAsync(createCitizen.ClassId);
        if (schoolClass is null)
        {
            return null;
        }

        _db.Citizens.Add(citizen);
        await _db.SaveChangesAsync();

        return new GetCitizenResponseDTO
        {
            UserId = citizen.Id,
            ClassId = citizen.ClassId,
            Username = citizen.Username,
            FirstName = citizen.FirstName,
            LastName = citizen.LastName,
        };
    }

    public async Task<GetCitizenResponseDTO?> PatchCitizenAsync(int citizenId, UpdateCitizenRequestDTO updateCitizen)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);

        if (citizen is null)
        {
            return null;
        }

        if (updateCitizen.ClassId is not null)
        {
            var schoolClass = await _db.Classes.FindAsync(updateCitizen.ClassId.Value);
            if (schoolClass is null)
            {
                return null;
            }
            citizen.ClassId = updateCitizen.ClassId.Value;
        }

        if (updateCitizen.FirstName is not null) citizen.FirstName = updateCitizen.FirstName;
        if (updateCitizen.LastName is not null) citizen.LastName = updateCitizen.LastName;
        if (updateCitizen.Username is not null) citizen.Username = updateCitizen.Username;

        await _db.SaveChangesAsync();

        return new GetCitizenResponseDTO
        {
            UserId = citizen.Id,
            ClassId = citizen.ClassId,
            FirstName = citizen.FirstName,
            LastName = citizen.LastName,
            Username = citizen.Username,
        };
    }

    public async Task<bool> DeleteCitizenAsync(int citizenId)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);
        if (citizen is not null)
        {
            _db.Citizens.Remove(citizen);
            await _db.SaveChangesAsync();
            return true;
        }
        else
        {
            return false;
        }
    }
}