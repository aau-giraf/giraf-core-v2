using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.X86;
using System.Security.Cryptography.X509Certificates;

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

    public async Task<ResponseGetNonCitizenDTO?> PatchNonCitizenAsync(int nonCitizenId, RequestPatchNonCitizenDTO requestPatchNonCitizenDTO)
    {
        //----- Changes to NonCitizen relation -----\\
        var nonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

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
        if (requestPatchNonCitizenDTO.NewPassword is not null && BCrypt.Net.BCrypt.Verify(requestPatchNonCitizenDTO.OldPassword, nonCitizen!.Password))
        {
            nonCitizen!.Password = BCrypt.Net.BCrypt.HashPassword(requestPatchNonCitizenDTO.NewPassword, 12);
        }

        // Save changes made to nonCitizen relation
        await _db.SaveChangesAsync();

        //----- Changes to userRole relation -----\\
        if (requestPatchNonCitizenDTO.UpdatedRoleOrganizationTuples is not null)
        {
            // Find the current list of userRoles tied to the nonCitizen
            var userRoles = await _db.UserRoles.Where(ur => ur.NonCitizenId == nonCitizenId).ToListAsync();

            /* Add case:
                For each entry in updatedRoleOrganizationTuples: If said entry does not already exist as a UserRole
                Then Add it as a UserRole to the db 
            */
            foreach ((RoleType, int) updatedRoleOrganizationTuple in requestPatchNonCitizenDTO.UpdatedRoleOrganizationTuples!)
            {
                // Check if there exists and userRole corresponding to element in 
                var userRoleExists = userRoles.Any(
                    ur => ur.NonCitizenId == nonCitizenId &&
                    ur.Role == updatedRoleOrganizationTuple.Item1 &&
                    ur.OrganizationId == updatedRoleOrganizationTuple.Item2);

                // If a userRoleExists is false -> 
                if (userRoleExists is false)
                {
                    UserRole createdUserRole = new UserRole
                    {
                        NonCitizenId = nonCitizenId,
                        NonCitizen = nonCitizen,
                        Role = updatedRoleOrganizationTuple.Item1,
                        OrganizationId = updatedRoleOrganizationTuple.Item2,
                        Organization = await _db.Organizations.FindAsync(updatedRoleOrganizationTuple.Item2)
                    };

                    _db.UserRoles.Add(createdUserRole);
                    await _db.SaveChangesAsync();
                }
            }

            /* Remove case:
                For each element in userRoles: If said element is not part of the new updatedRoleOrganizationTuples
                Then Remove that UserRole from the db 
            */
            foreach (UserRole userRole in userRoles)
            {
                var UpdatedRoleOrganizationTupleContains = requestPatchNonCitizenDTO.UpdatedRoleOrganizationTuples.Any(
                    urot => urot.Item1 == userRole.Role &&
                    urot.Item2 == userRole.OrganizationId
                );

                if (UpdatedRoleOrganizationTupleContains is false)
                {
                    _db.UserRoles.Remove(userRole);
                    await _db.SaveChangesAsync();
                }
            }
        }


        // Find the updated nonCitizen in db
        var updatedNonCitizen = await _db.NonCitizens.FindAsync(nonCitizenId);

        if (updatedNonCitizen is null)
        {
            return null;
        }

        ResponseGetNonCitizenDTO responseNonCitizen = new ResponseGetNonCitizenDTO
        {
            Id = updatedNonCitizen!.Id,
            FirstName = updatedNonCitizen.FirstName!,
            LastName = updatedNonCitizen.LastName!,
            Email = nonCitizen!.Email!,
            Username = updatedNonCitizen.Username!,
            Roles = updatedNonCitizen.Roles,
            Guardianships = updatedNonCitizen.Guardianships
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

    public async Task<ResponseGetNonCitizenDTO?> PostNonCitizenAsync(RequestPostNonCitizenDTO requestPostNonCitizenDTO)
    {
        // Get all roles from RoleOrganizationTuples
        List<RoleType> roleTypes = new List<RoleType>();
        foreach ((RoleType, int) roleOrganizationTuples in requestPostNonCitizenDTO.RoleOrganizationTuples)
        {
            roleTypes.Add(roleOrganizationTuples.Item1);
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

            // Add the nonCitizens new UserRole to the db
            foreach ((RoleType, int) roleOrganizationTuples in requestPostNonCitizenDTO.RoleOrganizationTuples)
            {
                UserRole createdUserRole = new UserRole()
                {
                    Role = roleOrganizationTuples.Item1,
                    NonCitizenId = createdNonCitizen.Id,
                    NonCitizen = createdNonCitizen,
                    OrganizationId = roleOrganizationTuples.Item2,
                    Organization = await _db.Organizations.FindAsync(roleOrganizationTuples.Item2)
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
            BCrypt.Net.BCrypt.Verify(requestPostNonCitizenDTO.Password, nc.Password) == true
        );

        if (nonCitizen is null)
        {
            return null;
        }

        ResponseGetNonCitizenDTO responseNonCitizen = new ResponseGetNonCitizenDTO
        {
            Id = nonCitizen!.Id,
            FirstName = requestPostNonCitizenDTO.FirstName!,
            LastName = requestPostNonCitizenDTO.LastName!,
            Email = requestPostNonCitizenDTO.Email!,
            Username = requestPostNonCitizenDTO.Username!,
            Roles = nonCitizen.Roles,
            Guardianships = nonCitizen.Guardianships
        };

        return responseNonCitizen;
    }

}