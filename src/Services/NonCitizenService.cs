using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class NonCitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // ---------- /nonCitizens ---------- \\

    // Fetch the NonCitizen by NonCitizenId from Route parameter
    public async Task<GetNonCitizenResponseDTO?> GetNonCitizenByIdAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        GetNonCitizenResponseDTO responseNonCitizen = new GetNonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen.Email,
            Username = nonCitizen.Username
        };

        return responseNonCitizen;
    }

    public async Task<(HttpResponseType, GetNonCitizenResponseDTO?)> PatchNonCitizenAsync(int nonCitizenId, PatchNonCitizenRequestDTO patchNonCitizen)
    {
        //----- Changes to NonCitizen relation -----\\
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpResponseType.NotFound, null);
        }

        if (patchNonCitizen.FirstName is not null)
        {
            nonCitizen!.FirstName = patchNonCitizen.FirstName;
        }
        if (patchNonCitizen.LastName is not null)
        {
            nonCitizen!.LastName = patchNonCitizen.LastName;
        }
        if (patchNonCitizen.Username is not null)
        {
            nonCitizen!.Username = patchNonCitizen.Username;
        }
        if (patchNonCitizen.Email is not null && await _db.NonCitizens.AnyAsync(nc => nc.Email == patchNonCitizen.Email) == false)
        {
            nonCitizen!.Email = patchNonCitizen.Email;
        }
        else if (patchNonCitizen.Email is not null && await _db.NonCitizens.AnyAsync(nc => nc.Email == patchNonCitizen.Email) == true)
        {
            return (HttpResponseType.Conflict, null);
        }
        if (patchNonCitizen.NewPassword is not null && BCrypt.Net.BCrypt.Verify(patchNonCitizen.OldPassword, nonCitizen!.Password) == true)
        {
            nonCitizen!.Password = BCrypt.Net.BCrypt.HashPassword(patchNonCitizen.NewPassword, 12);
        }
        else if (patchNonCitizen.NewPassword is not null && BCrypt.Net.BCrypt.Verify(patchNonCitizen.OldPassword, nonCitizen.Password) == false)
        {
            return (HttpResponseType.Unauthorized, null);
        }


        // Save changes made to nonCitizen relation
        await _db.SaveChangesAsync();

        GetNonCitizenResponseDTO responseNonCitizen = new GetNonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen.Id,
            FirstName = nonCitizen.FirstName,
            LastName = nonCitizen.LastName,
            Email = nonCitizen!.Email,
            Username = nonCitizen.Username
        };

        return (HttpResponseType.Ok, responseNonCitizen);
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

    public async Task<(HttpResponseType, GetNonCitizenResponseDTO?)> PostNonCitizenAsync(PostNonCitizenRequestDTO postNonCitizen)
    {
        var isNotUnique = await _db.NonCitizens.AnyAsync(nc => nc.Email == postNonCitizen.Email);

        if (isNotUnique == true)
        {
            return (HttpResponseType.Conflict, null);
        }

        if (
            postNonCitizen.FirstName is not null &&
            postNonCitizen.LastName is not null &&
            postNonCitizen.Email is not null &&
            postNonCitizen.Username is not null &&
            postNonCitizen.Password is not null
        )
        {
            // Create new instance of NonCitizen
            NonCitizen createdNonCitizen = new NonCitizen
            {
                FirstName = postNonCitizen.FirstName,
                LastName = postNonCitizen.LastName,
                Username = postNonCitizen.Username,
                Password = BCrypt.Net.BCrypt.HashPassword(postNonCitizen.Password, 12),
                Email = postNonCitizen.Email
            };

            // Add the new nonCitizen instance to the db
            _db.NonCitizens.Add(createdNonCitizen);
            await _db.SaveChangesAsync();
        }

        // Find created nonCitizen in db
        var nonCitizen = await _db.NonCitizens.FirstOrDefaultAsync(
            nc => nc.Username == postNonCitizen.Username &&
            nc.FirstName == postNonCitizen.FirstName &&
            nc.LastName == postNonCitizen.LastName &&
            nc.Email == postNonCitizen.Email
        );

        if (nonCitizen is null)
        {
            return (HttpResponseType.NotFound, null);
        }

        GetNonCitizenResponseDTO responseNonCitizen = new GetNonCitizenResponseDTO
        {
            NonCitizenId = nonCitizen!.Id,
            FirstName = nonCitizen.FirstName!,
            LastName = nonCitizen.LastName!,
            Email = nonCitizen.Email!,
            Username = nonCitizen.Username!
        };

        return (HttpResponseType.Ok, responseNonCitizen);
    }


    // ---------- /userRoles ---------- \\

    public async Task<GetNonCitizenUserRolesResponseDTO?> GetUserRolesAsync(int nonCitizenId)
    {
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return null;
        }

        var userRoles = await _db.UserRoles.Where(ur => ur.NonCitizenId == nonCitizenId).ToListAsync();

        // Select only the needed from userRole or else it would eagerly fetch FK fields. 
        ICollection<string> roleOrganizationTuples = [];

        foreach (EmployeeRole userRole in userRoles)
        {
            var roleString = userRole.Role.ToString();
            var organizationIdText = userRole.OrganizationId.ToString();
            var roleOrganizationText = roleString + "|" + organizationIdText;
            roleOrganizationTuples.Add(roleOrganizationText);
        }

        return new GetNonCitizenUserRolesResponseDTO
        {
            NonCitizenId = nonCitizen!.Id,
            RoleOrganizationTuples = roleOrganizationTuples
        };
    }

    public async Task<bool> DeleteUserRoleAsync(int nonCitizenId, NonCitizenUserRoleRequestDTO nonCitizenUserRole)
    {
        if (await _db.UserRoles.AnyAsync(ur =>
             ur.NonCitizenId == nonCitizenId &&
             ur.OrganizationId == nonCitizenUserRole.OrganizationId &&
             ur.Role == nonCitizenUserRole.Role
             ))
        {
            var userRole = await _db.UserRoles
            .FindAsync(nonCitizenId, nonCitizenUserRole.Role, nonCitizenUserRole.OrganizationId);

            if (
                userRole!.NonCitizenId == nonCitizenId &&
                userRole.OrganizationId == nonCitizenUserRole.OrganizationId &&
                userRole.Role == nonCitizenUserRole.Role &&
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

    public async Task<(HttpResponseType, GetNonCitizenUserRolesResponseDTO?)> PostUserRoleAsync(int nonCitizenId, NonCitizenUserRoleRequestDTO nonCitizenUserRole)
    {

        // Check nonCitizen exists
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpResponseType.NotFound, null);
        }

        // Check organization exists
        var organization = await _db.Organizations.FindAsync(nonCitizenUserRole.OrganizationId);
        if (organization is null)
        {
            return (HttpResponseType.NotFound, null);
        }

        // Check if userRole exists
        var userRoleExists = await _db.UserRoles.AnyAsync(ur =>
             ur.NonCitizenId == nonCitizenId &&
             ur.Role == nonCitizenUserRole.Role &&
             ur.OrganizationId == nonCitizenUserRole.OrganizationId
            );

        if (userRoleExists is true)
        {
            return (HttpResponseType.Conflict, null);
        }

        var createdUserRole = new EmployeeRole()
        {
            Role = nonCitizenUserRole.Role,
            NonCitizenId = nonCitizen.Id,
            NonCitizen = nonCitizen,
            OrganizationId = nonCitizenUserRole.OrganizationId,
            Organization = await _db.Organizations.FindAsync(nonCitizenUserRole.OrganizationId)
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
        return (HttpResponseType.Ok, new GetNonCitizenUserRolesResponseDTO()
        {
            NonCitizenId = nonCitizen.Id,
            RoleOrganizationTuples = roleOrganizationTuples
        });
    }

    // ---------- /guardianships ---------- \\
    public async Task<GetGuardianshipsResponseDTO?> GetGuardianshipsAsync(int nonCitizenId)
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

        return new GetGuardianshipsResponseDTO
        {
            CitizenIds = citizens,
            NonCitizenId = nonCitizen.Id
        };
    }

    public async Task<bool> DeleteGuardianshipAsync(int nonCitizenId, GuardianshipRequestDTO guardianshipRequest)
    {
        if (await _db.Guardianships.AnyAsync(gs =>
             gs.CitizenId == guardianshipRequest.CitizenId &&
             gs.GuardianId == nonCitizenId
             ))
        {
            var guardianship = await _db.Guardianships
            .FindAsync(guardianshipRequest.CitizenId, nonCitizenId);

            if (
                guardianship!.GuardianId == nonCitizenId &&
                guardianship.CitizenId == guardianshipRequest.CitizenId &&
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

    public async Task<(HttpResponseType, GetGuardianshipsResponseDTO?)> PostGuardianshipAsync(int nonCitizenId, GuardianshipRequestDTO guardianshipRequest)
    {

        // Check nonCitizen exists
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (nonCitizen is null)
        {
            return (HttpResponseType.NotFound, null);
        }

        var citizen = await _db.Citizens.AnyAsync(c => c.Id == guardianshipRequest.CitizenId);

        if (citizen == false)
        {
            return (HttpResponseType.NotFound, null);
        }

        // Check if guardianship exists
        var guardianshipExists = await _db.Guardianships.AnyAsync(
            gs => gs.CitizenId == guardianshipRequest.CitizenId &&
            gs.GuardianId == nonCitizenId);

        if (guardianshipExists is true)
        {
            return (HttpResponseType.Conflict, null);
        }

        var createdGuardianship = new Guardianship()
        {
            CitizenId = guardianshipRequest.CitizenId,
            Citizen = await _db.Citizens.FindAsync(guardianshipRequest.CitizenId),
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
        return (HttpResponseType.Ok, new GetGuardianshipsResponseDTO()
        {
            NonCitizenId = nonCitizen.Id,
            CitizenIds = citizens
        });
    }


}