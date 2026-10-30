using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace giraf_core_v2.Services;

public class CitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<GetCitizenDTO?> GetCitizenAsync(int userId)
    {
        var citizen = await _db.Citizens.FindAsync(userId);

        if (citizen is null)
        {
            return null;
        }

        GetCitizenDTO getCitizenDto = new GetCitizenDTO
        {
            UserId = citizen.Id,
            ClassId = citizen.ClassId,
            FirstName = citizen.FirstName,
            LastName = citizen.LastName,
            Username = citizen.Username,
        };

        return getCitizenDto;
    }

    public async Task<CreateCitizenDTO?> CreateCitizenAsync(CreateCitizenDTO dto)
    {
        var citizen = new Citizen
        {
            ClassId = dto.ClassId,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Password = BCrypt.Net.BCrypt.HashPassword(dto.Password, 12),
            Username = dto.Username,
        };
        
        var schoolClass = await _db.Classes.FindAsync(dto.ClassId);
        if (schoolClass is null)
        {
            return null;
        }

        _db.Citizens.Add(citizen);
        await _db.SaveChangesAsync();

        return new CreateCitizenDTO
        {
            ClassId = citizen.ClassId,
            Username = citizen.Username,
            FirstName = citizen.FirstName,
            LastName = citizen.LastName,
        };
    }

    public async Task<GetCitizenDTO?> PatchCitizenAsync(int userId, UpdateCitizenDTO dto)
    {
        var citizen = await _db.Citizens.FindAsync(userId);

        if (citizen is null)
        {
            return null;
        }

        if (dto.ClassId is not null)
        {
            var schoolClass = await _db.Classes.FindAsync(dto.ClassId.Value);
            if (schoolClass is null)
            {
                return null;
            }
            citizen.ClassId = dto.ClassId.Value;
        }
        
        if (dto.FirstName is not null) citizen.FirstName = dto.FirstName;
        if (dto.LastName is not null) citizen.LastName = dto.LastName;
        if (dto.Username is not null) citizen.Username = dto.Username;

        await _db.SaveChangesAsync();

        return new GetCitizenDTO
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