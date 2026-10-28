using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class NonCitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // Fetch the NonCitizen by citizenId from Route parameter
    public async Task<ResponseGetNonCitizenDTO?> GetNonCitizenByIdAsync(int citizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(citizenId);

        var role = await _db.UserRoles.FindAsync(citizenId);

        ResponseGetNonCitizenDTO responseNonCitizen = new ResponseGetNonCitizenDTO
        {
            Id = nonCitizen!.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen.Email,
            Username = nonCitizen.Username,
            Roles = nonCitizen.Roles
        };

        return responseNonCitizen;
    }

    public async Task<ResponseGetNonCitizenDTO?> PatchNonCitizenAsync(int nonCitizenId, RequestPatchNonCitizenDTO requestPatchNonCitizenDTO)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        // Ensure that the new values are in fact new. 
        if (
            nonCitizen!.FirstName == requestPatchNonCitizenDTO.FirstName ||
            nonCitizen.LastName == requestPatchNonCitizenDTO.LastName ||
            nonCitizen.Email == requestPatchNonCitizenDTO.Email ||
            nonCitizen.Username == requestPatchNonCitizenDTO.Username
        )
        {
            return null;
        }

        ResponseGetNonCitizenDTO responseNonCitizen = new ResponseGetNonCitizenDTO
        {
            Id = nonCitizen!.Id,
            FirstName = requestPatchNonCitizenDTO.FirstName,
            LastName = requestPatchNonCitizenDTO.LastName,
            Email = requestPatchNonCitizenDTO.Email,
            Username = requestPatchNonCitizenDTO.Username,
            Roles = nonCitizen.Roles
        };

        return responseNonCitizen;
    }

    public async Task<bool> DeleteNonCitizenAsync(int nonCitizenId)
    {
        // Fetch nonCitizen and delete 
        if (await _db.NonCitizens.AnyAsync(nc => nc.Id == nonCitizenId))
        {
            var nonCitizen = await _db.NonCitizens
            .FindAsync(nonCitizenId);

            if (nonCitizen!.Id == nonCitizenId && nonCitizen is not null)
            {
                _db.NonCitizens.Remove(nonCitizen);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }


}