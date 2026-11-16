using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<OrganizationResponseDTO>> GetAllOrganizationsAsync()
    {
        return await _db.Organizations
            .Select(o => new OrganizationResponseDTO
            {
                OrganizationId = o.Id,
                Name = o.Name,
            })
            .ToListAsync();
    }

    public async Task<OrganizationResponseDTO?> GetOrganizationByIdAsync(int organizationId)
    {
        return await _db.Organizations
        .Where(o => o.Id == organizationId)
        .Select(o => new OrganizationResponseDTO
        {
            OrganizationId = o.Id,
            Name = o.Name,
        })
        .FirstOrDefaultAsync();
    }

    // Delete organization by org_id from Route parameter
    public async Task<bool> DeleteOrganizationAsync(int orgId)
    {
        // Fetch organization and delete it
        if (await _db.Organizations.AnyAsync(o => o.Id == orgId))
        {
            var organization = await _db.Organizations
            .FindAsync(orgId);

            if (organization!.Id == orgId && organization is not null)
            {
                _db.Organizations.Remove(organization);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

        // Attempt to fetch the class.
        var organizationClass = await _db.Classes
            .Where(c => c.OrganizationId == organizationId && c.Id == classId)
            .Select(c => new ClassResponseDTO
            {
                ClassId = c.Id,
                Name = c.Name,
                OrganizationId = c.OrganizationId,
            })
            .FirstOrDefaultAsync();

        // Check that the class was found.
        if (organizationClass is null)
        {
            return (OrganizationServiceResult.ClassNotFound, null);
        }

        return (OrganizationServiceResult.Success, organizationClass);
    }

    public async Task<(OrganizationServiceResult, List<ClassResponseDTO>?)> GetAllClassesInOrganizationAsync(int organizationId)
    {
        // Check that the specified organization exists.
        bool organizationExists = await _db.Organizations
            .AnyAsync(o => o.Id == organizationId);

        if (!organizationExists)
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        // Fetch the classes and return the list.
        return (OrganizationServiceResult.Success, await _db.Classes
            .Where(c => c.OrganizationId == organizationId)
            .Select(c => new ClassResponseDTO
            {
                ClassId = c.Id,
                Name = c.Name,
                OrganizationId = c.OrganizationId,
            })
            .ToListAsync());
    }

    public async Task<(OrganizationServiceResult, ClassResponseDTO?)> CreateClassInOrganizationAsync(CreateClassDTO createClassDto, int organizationId)
    {
        // Check that the specified organization exists.
        bool organizationExists = await _db.Organizations
            .AnyAsync(o => o.Id == organizationId);

        if (!organizationExists)
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        Class newClass = new()
        {
            Name = createClassDto.Name.ToLower(),
            OrganizationId = organizationId,
        };

        // Return null if proposed class already exists.
        if (await _db.Classes.AnyAsync(c => c.Name.ToLower().Equals(newClass.Name) && c.OrganizationId == organizationId))
        {
            return (OrganizationServiceResult.ClassAlreadyExists, null);
        }

        _db.Classes.Add(newClass);

        await _db.SaveChangesAsync();

        return (OrganizationServiceResult.Success, new ClassResponseDTO
        {
            ClassId = newClass.Id,
            Name = newClass.Name,
            OrganizationId = newClass.OrganizationId,
        });
    }

    public async Task<(OrganizationServiceResult, OrganizationResponseDTO?)> CreateOrganizationAsync(OrganizationDTO createOrganizationDto)
    {
        var newOrganization = new Organization
        {
            Name = createOrganizationDto.Name.ToLower(),
        };

        // Return null if proposed organization already exists.
        if (await _db.Organizations.AnyAsync(o => o.Name.ToLower().Equals(newOrganization.Name)))
        {
            return (OrganizationServiceResult.OrganizationAlreadyExists, null);
        }

        _db.Organizations.Add(newOrganization);

        await _db.SaveChangesAsync();

        return (OrganizationServiceResult.Success, new OrganizationResponseDTO
        {
            OrganizationId = newOrganization.Id,
            Name = newOrganization.Name,
        });
    }

    {
        // Check if the name specified in the requestbody is already taken
        bool checkNameAlreadyExist = await _db.Organizations.AnyAsync(o => o.Name == createOrganization.Name);

        // Create new instance of Organization
        Organization createdOrganization = new Organization
        {
            Name = createOrganization.Name,
        };

        // Insert the new instance tuple into the db.
        if (createdOrganization.Name is not null && checkNameAlreadyExist is false)
        {
            _db.Organizations.Add(createdOrganization);
            await _db.SaveChangesAsync();
        }

        // Fetch the new organization in the db. 
        // The db makes the organization Id on insert, which is required in the response body
        // It is therefore we insert it in the db, and then fetches that same entry now containing id. 
        Organization newOrganization = _db.Organizations.FirstOrDefault(o => o.Name == createdOrganization.Name)!;

        if (newOrganization is null)
        {
            return null;
        }

        // Return correctly formatted response
        ResponseGetOrganizationDTO responseOrganization = new ResponseGetOrganizationDTO
        {
            Id = newOrganization.Id,
            Name = newOrganization.Name,
        };

        return responseOrganization;

    }

    public async Task<ResponseGetOrganizationDTO?> UpdateOrganizationAsync(RequestCreateOrganizationDTO updateOrganization, int orgId)
    {
        // Find the organization in db
        var organization = await _db.Organizations.FindAsync(orgId);

        if (organization is null)
        {
            return null;
        }

        // Check if the name specified in the request is already taken, including if the name is not changed it is taken. 
        bool nameAlreadyExist = await _db.Organizations.AnyAsync(o => o.Name == updateOrganization.Name);

        if (organization!.Id == orgId && nameAlreadyExist is false && updateOrganization.Name is not null)
        {
            organization.Name = updateOrganization.Name;
        }

        // Save the updated organization in db. 
        await _db.SaveChangesAsync();

        // Find the updated organization in db
        var updatedOrganization = await _db.Organizations.FindAsync(orgId);

        if (updatedOrganization is null)
        {
            return null;
        }

        // Return correctly formatted response
        ResponseGetOrganizationDTO responseOrganization = new ResponseGetOrganizationDTO
        {
            Id = updatedOrganization!.Id,
            Name = updatedOrganization.Name,
        };

        return responseOrganization;
    }

    public async Task<bool> DeleteUserRoleAsync(int organizationId, int nonCitizenId, RoleType role)
    {
        /*Consider direct deletion instead of 2 round trips. Implications of EF Core change tracking unknown.
        * Be aware of composite key ordering of UserRole. */
        if (await _db.UserRoles.FindAsync(nonCitizenId, role, organizationId) is UserRole userRole)
        {
            _db.UserRoles.Remove(userRole);
            await _db.SaveChangesAsync();
            //Deletion occured, indicate for HTTP response
            return true;
        }
        //Corresponding row didn't exist, no deletion occurred, indicate for HTTP response
        return false;
    }

    public async Task<(int, UserRole?)> CreateOrUpdateUserRole(int organizationId, int nonCitizenId, RoleType role)
    {
        //CASE 1: organization, non-citizen, role each exist seperately. (Assumed to minimize reads, most frequent case)
        bool createdUserRole = false;
        var userRole = await _db.UserRoles.FindAsync(nonCitizenId, role, organizationId);

        // 1.A: the UserRole tuple does not exist. Create it
        if (userRole is null)
        {
            //Create new object
            userRole = new UserRole
            {
                NonCitizenId = nonCitizenId,
                Role = role,
                OrganizationId = organizationId
            };

            //Add it to change tracking
            _db.UserRoles.Add(userRole);

            //Indicate Successfull(Created) case 1.A occurred
            createdUserRole = true;
        }

        // Whether 1.A(created) | 1.B(updated) - Future properties can be set here, before try-cactch block 

        try
        {
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException) when (createdUserRole)
        {
            /* Case 2:  Either organization, non-citizen or role did not exist. Client error response.
             * Potentially check wether organization or user failed to provide more detailed response */
            return (-1, null);
        }

        //For successfull cases, distuinguish between creation & update
        return createdUserRole ? (1, userRole) : (0, null);
    }
}