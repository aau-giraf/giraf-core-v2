using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class NonCitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // ---------- /nonCitizens ---------- \\

    // Fetch the NonCitizen by NonCitizenId from Route parameter
    public async Task<NonCitizenResponseDTO?> GetNonCitizenByIdAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        NonCitizenResponseDTO responseNonCitizen = new NonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen.Email,
            Username = nonCitizen.Username
        };

        return responseNonCitizen;
    }

    public async Task<(HttpReponseType, NonCitizenResponseDTO?)> PatchNonCitizenAsync(int nonCitizenId, PatchNonCitizenRequestDTO requestPatchNonCitizenDTO)
    {
        //----- Changes to NonCitizen relation -----\\
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpReponseType.NotFound, null);
        }

        if (requestPatchNonCitizenDTO.FirstName is not null)
        {
            nonCitizen!.FirstName = requestPatchNonCitizenDTO.FirstName;
        }
        if (requestPatchNonCitizenDTO.LastName is not null)
        {
            nonCitizen!.LastName = requestPatchNonCitizenDTO.LastName;
        }
        if (requestPatchNonCitizenDTO.Username is not null)
        {
            nonCitizen!.Username = requestPatchNonCitizenDTO.Username;
        }
        if (requestPatchNonCitizenDTO.Email is not null && await _db.NonCitizens.AnyAsync(nc => nc.Email == requestPatchNonCitizenDTO.Email) == false)
        {
            nonCitizen!.Email = requestPatchNonCitizenDTO.Email;
        }
        else if (requestPatchNonCitizenDTO.Email is not null && await _db.NonCitizens.AnyAsync(nc => nc.Email == requestPatchNonCitizenDTO.Email) == true)
        {
            return (HttpReponseType.Conflict, null);
        }
        if (requestPatchNonCitizenDTO.NewPassword is not null && BCrypt.Net.BCrypt.Verify(requestPatchNonCitizenDTO.OldPassword, nonCitizen!.Password) == true)
        {
            nonCitizen!.Password = BCrypt.Net.BCrypt.HashPassword(requestPatchNonCitizenDTO.NewPassword, 12);
        }
        else if (requestPatchNonCitizenDTO.NewPassword is not null && BCrypt.Net.BCrypt.Verify(requestPatchNonCitizenDTO.OldPassword, nonCitizen.Password) == false)
        {
            return (HttpReponseType.Unauthorized, null);
        }


        // Save changes made to nonCitizen relation
        await _db.SaveChangesAsync();

        NonCitizenResponseDTO responseNonCitizen = new NonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen!.Email,
            Username = nonCitizen.Username
        };

        return (HttpReponseType.Ok, responseNonCitizen);
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

    public async Task<(HttpReponseType, NonCitizenResponseDTO?)> PostNonCitizenAsync(PostNonCitizenRequestDTO requestPostNonCitizenDTO)
    {
        var isNotUnique = await _db.NonCitizens.AnyAsync(nc => nc.Email == requestPostNonCitizenDTO.Email);

        if (isNotUnique == true)
        {
            return (HttpReponseType.Conflict, null);
        }

        if (
            requestPostNonCitizenDTO.FirstName is not null &&
            requestPostNonCitizenDTO.LastName is not null &&
            requestPostNonCitizenDTO.Email is not null &&
            requestPostNonCitizenDTO.Username is not null &&
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
        }

        // Find created nonCitizen in db
        var nonCitizen = await _db.NonCitizens.FirstOrDefaultAsync(
            nc => nc.Username == requestPostNonCitizenDTO.Username &&
            nc.FirstName == requestPostNonCitizenDTO.FirstName &&
            nc.LastName == requestPostNonCitizenDTO.LastName &&
            nc.Email == requestPostNonCitizenDTO.Email
        );

        if (nonCitizen is null)
        {
            return (HttpReponseType.NotFound, null);
        }

        NonCitizenResponseDTO responseNonCitizen = new NonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen!.Id,
            FirstName = nonCitizen.FirstName!,
            LastName = nonCitizen.LastName!,
            Email = nonCitizen.Email!,
            Username = nonCitizen.Username!
        };

        return (HttpReponseType.Ok, responseNonCitizen);
    }


    // ---------- /userRoles ---------- \\

    public async Task<NonCitizenUserRolesResponseDTO?> GetUserRolesAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        var userRoles = await _db.UserRoles.Where(ur => ur.NonCitizenId == nonCitizenId).ToListAsync();

        // Select only the needed from userRole or else it would eagerly fetch FK fields. 
        ICollection<string> roleOrganizationTuples = [];

        foreach (UserRole userRole in userRoles)
        {
            var roleString = userRole.Role.ToString();
            var organizationIdText = userRole.OrganizationId.ToString();
            var roleOrganizationText = roleString + "|" + organizationIdText;
            roleOrganizationTuples.Add(roleOrganizationText);
        }

        return new NonCitizenUserRolesResponseDTO
        {
            NonCitizenId = nonCitizen!.Id,
            RoleOrganizationTuples = roleOrganizationTuples
        };
    }

    public async Task<bool> DeleteUserRoleAsync(int nonCitizenId, NonCitizenUserRoleRequestDTO requestNonCitizenUserRoleDTO)
    {
        if (await _db.UserRoles.AnyAsync(ur =>
             ur.NonCitizenId == nonCitizenId &&
             ur.OrganizationId == requestNonCitizenUserRoleDTO.OrganizationId &&
             ur.Role == requestNonCitizenUserRoleDTO.Role
             ))
        {
            var userRole = await _db.UserRoles
            .FindAsync(nonCitizenId, requestNonCitizenUserRoleDTO.Role, requestNonCitizenUserRoleDTO.OrganizationId);

            if (
                userRole!.NonCitizenId == nonCitizenId &&
                userRole.OrganizationId == requestNonCitizenUserRoleDTO.OrganizationId &&
                userRole.Role == requestNonCitizenUserRoleDTO.Role &&
                userRole is not null
                )
            {
                _db.UserRoles.Remove(userRole);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<(HttpReponseType, NonCitizenUserRolesResponseDTO?)> PostUserRoleAsync(int nonCitizenId, NonCitizenUserRoleRequestDTO requestNonCitizenUserRoleDTO)
    {

        // Check nonCitizen exists
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpReponseType.NotFound, null);
        }

        // Check organization exists
        var organization = await _db.Organizations.FindAsync(requestNonCitizenUserRoleDTO.OrganizationId);
        if (organization is null)
        {
            return (HttpReponseType.NotFound, null);
        }

        // Check if userRole exists
        var userRoleExists = await _db.UserRoles.AnyAsync(ur =>
             ur.NonCitizenId == nonCitizenId &&
             ur.Role == requestNonCitizenUserRoleDTO.Role &&
             ur.OrganizationId == requestNonCitizenUserRoleDTO.OrganizationId
            );

        if (userRoleExists is true)
        {
            return (HttpReponseType.Conflict, null);
        }

        var createdUserRole = new UserRole()
        {
            Role = requestNonCitizenUserRoleDTO.Role,
            NonCitizenId = nonCitizen.Id,
            NonCitizen = nonCitizen,
            OrganizationId = requestNonCitizenUserRoleDTO.OrganizationId,
            Organization = await _db.Organizations.FindAsync(requestNonCitizenUserRoleDTO.OrganizationId)
        };

        _db.UserRoles.Add(createdUserRole);
        await _db.SaveChangesAsync();

        // Find the updated list of userRoles
        var userRoles = await _db.UserRoles.Where(ur => ur.NonCitizenId == nonCitizenId).ToListAsync();

        // Select only the needed from userRole
        List<string> roleOrganizationTuples = [];

        foreach (var userRole in userRoles)
        {
            var roleString = userRole.Role.ToString();
            var organizationIdText = userRole.OrganizationId.ToString();
            var roleOrganizationText = roleString + "|" + organizationIdText;
            roleOrganizationTuples.Add(roleOrganizationText);
        }

        // Return in correct format
        return (HttpReponseType.Ok, new NonCitizenUserRolesResponseDTO()
        {
            NonCitizenId = nonCitizen.Id,
            RoleOrganizationTuples = roleOrganizationTuples
        });
    }

    // ---------- /guardianships ---------- \\
    public async Task<GuardianshipsResponseDTO?> GetGuardianshipsAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        var guardianships = await _db.Guardianships.Where(gs => gs.GuardianId == nonCitizenId).ToListAsync();

        // Select the citizens belonging to the specified guardian
        List<int> citizens = [];
        foreach (var guardian in guardianships)
        {
            citizens.Add(guardian.CitizenId);
        }

        return new GuardianshipsResponseDTO
        {
            CitizenIds = citizens,
            NonCitizenId = nonCitizen.Id
        };
    }

    public async Task<bool> DeleteGuardianshipAsync(int nonCitizenId, GuardianshipRequestDTO requestGuardianshipDTO)
    {
        if (await _db.Guardianships.AnyAsync(gs =>
             gs.CitizenId == requestGuardianshipDTO.CitizenId &&
             gs.GuardianId == nonCitizenId
             ))
        {
            var guardianship = await _db.Guardianships
            .FindAsync(requestGuardianshipDTO.CitizenId, nonCitizenId);

            if (
                guardianship!.GuardianId == nonCitizenId &&
                guardianship.CitizenId == requestGuardianshipDTO.CitizenId &&
                guardianship is not null
                )
            {
                _db.Guardianships.Remove(guardianship);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<(HttpReponseType, GuardianshipsResponseDTO?)> PostGuardianshipAsync(int nonCitizenId, GuardianshipRequestDTO requestGuardianshipDTO)
    {

        // Check nonCitizen exists
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpReponseType.NotFound, null);
        }

        var citizen = await _db.Citizens.AnyAsync(c => c.Id == requestGuardianshipDTO.CitizenId);

        if (citizen == false)
        {
            return (HttpReponseType.NotFound, null);
        }

        // Check if guardianship exists
        var guardianshipExists = await _db.Guardianships.AnyAsync(
            gs => gs.CitizenId == requestGuardianshipDTO.CitizenId &&
            gs.GuardianId == nonCitizenId);

        if (guardianshipExists is true)
        {
            return (HttpReponseType.Conflict, null);
        }

        var createdGuardianship = new Guardianship()
        {
            CitizenId = requestGuardianshipDTO.CitizenId,
            Citizen = await _db.Citizens.FindAsync(requestGuardianshipDTO.CitizenId),
            GuardianId = nonCitizenId,
            Guardian = nonCitizen
        };

        _db.Guardianships.Add(createdGuardianship);
        await _db.SaveChangesAsync();

        // Find the updated list of citizens
        var guardianships = await _db.Guardianships.Where(gs => gs.GuardianId == nonCitizenId).ToListAsync();

        // Select only the needed from guardianships
        List<int> citizens = [];

        foreach (var guardian in guardianships)
        {
            citizens.Add(guardian.CitizenId);
        }

        // Return in correct format
        return (HttpReponseType.Ok, new GuardianshipsResponseDTO()
        {
            NonCitizenId = nonCitizen.Id,
            CitizenIds = citizens
        });
    }


}