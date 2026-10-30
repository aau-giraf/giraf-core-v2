using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;

using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class NonCitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // Fetch the NonCitizen by NoncitizenId from Route parameter
    public async Task<ResponseGetNonCitizenDTO?> GetNonCitizenByIdAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        ResponseGetNonCitizenDTO responseNonCitizen = new ResponseGetNonCitizenDTO
        {
            Id = nonCitizen!.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen.Email,
            Username = nonCitizen.Username,
            Roles = nonCitizen.Roles,
            Guardianships = nonCitizen.Guardianships
        };

        return responseNonCitizen;
    }

    public async Task<ResponsePatchNonCitizenDTO?> PatchNonCitizenAsync(int nonCitizenId, RequestPatchNonCitizenDTO requestPatchNonCitizenDTO)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (requestPatchNonCitizenDTO.FirstName is not null)
        {
            nonCitizen!.FirstName = requestPatchNonCitizenDTO.FirstName;
        }
        if (requestPatchNonCitizenDTO.LastName is not null)
        {
            nonCitizen!.LastName = requestPatchNonCitizenDTO.LastName;
        }
        if (requestPatchNonCitizenDTO.Email is not null)
        {
            nonCitizen!.Email = requestPatchNonCitizenDTO.Email;
        }
        if (requestPatchNonCitizenDTO.Username is not null)
        {
            nonCitizen!.Username = requestPatchNonCitizenDTO.Username;
        }

        // Save changes.
        await _db.SaveChangesAsync();

        // Find the updated nonCitizen in db
        var updatedNonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (updatedNonCitizen is null)
        {
            return null;
        }

        ResponsePatchNonCitizenDTO responseNonCitizen = new ResponsePatchNonCitizenDTO
        {
            Id = updatedNonCitizen!.Id,
            FirstName = updatedNonCitizen.FirstName!,
            LastName = updatedNonCitizen.LastName!,
            Email = updatedNonCitizen.Email!,
            Username = updatedNonCitizen.Username!,
            Roles = updatedNonCitizen.Roles
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

    public async Task<ResponsePostNonCitizenDTO?> PostNonCitizenAsync(RequestPostNonCitizenDTO requestPostNonCitizenDTO)
    {
        // Variable used to check roleType
        var roleType = requestPostNonCitizenDTO.Roles.GetType();

        if (
            requestPostNonCitizenDTO.FirstName is not null &&
            requestPostNonCitizenDTO.LastName is not null &&
            requestPostNonCitizenDTO.Email is not null &&
            requestPostNonCitizenDTO.Username is not null &&
            requestPostNonCitizenDTO.Roles is not null &&
            roleType == typeof(RoleType) &&
            requestPostNonCitizenDTO.Password is not null
        )
        {
            // Create new instance of NonCitizen
            NonCitizen createdNonCitizen = new NonCitizen
            {
                FirstName = requestPostNonCitizenDTO.FirstName,
                LastName = requestPostNonCitizenDTO.LastName,
                Username = requestPostNonCitizenDTO.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(requestPostNonCitizenDTO.Password, 12),
                Email = requestPostNonCitizenDTO.Email
            };

            // Add the new nonCitizen instance to the db
            _db.NonCitizens.Add(createdNonCitizen);
            await _db.SaveChangesAsync();

            var organization = await _db.Organizations.FindAsync(requestPostNonCitizenDTO.OrganizationId);

            if (organization is null)
            {
                return null;
            }

            // Add the nonCitizens new UserRole to the db
            foreach (UserRole userRole in requestPostNonCitizenDTO.Roles)
            {
                UserRole createdUserRole = new UserRole()
                {
                    Role = userRole.Role,
                    NonCitizenId = createdNonCitizen.Id,
                    NonCitizen = createdNonCitizen,
                    OrganizationId = organization!.Id,
                    Organization = organization
                };

                _db.UserRoles.Add(createdUserRole);
                await _db.SaveChangesAsync();
            }
        }

        // Find created nonCitizen in db
        var nonCitizen = await _db.NonCitizens.FirstOrDefaultAsync(
            nc => nc.Username == requestPostNonCitizenDTO.Username &&
            nc.FirstName == requestPostNonCitizenDTO.FirstName &&
            nc.LastName == requestPostNonCitizenDTO.LastName &&
            nc.Email == requestPostNonCitizenDTO.Email &&
            nc.Roles == requestPostNonCitizenDTO.Roles
            );

        if (nonCitizen is null)
        {
            return null;
        }

        ResponsePostNonCitizenDTO responseNonCitizen = new ResponsePostNonCitizenDTO
        {
            Id = nonCitizen!.Id,
            FirstName = requestPostNonCitizenDTO.FirstName!,
            LastName = requestPostNonCitizenDTO.LastName!,
            Email = requestPostNonCitizenDTO.Email!,
            Username = requestPostNonCitizenDTO.Username!,
            Roles = nonCitizen.Roles
        };

        return responseNonCitizen;
    }

}