using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<GetOrganizationResponseDTO>> GetOrganizationsAsync()
    {
        // Fetch organizations
        var organizations = await _db.Organizations.ToListAsync();

        // List of organizationDTOs
        List<GetOrganizationResponseDTO> organizationsDtoList = new List<GetOrganizationResponseDTO>();

        foreach (Organization organization in organizations)
        {
            GetOrganizationResponseDTO responseOrganization = new GetOrganizationResponseDTO
            {
                OrganizationId = organization.Id,
                Name = organization.Name,
            };
            // Add each organization that has been converted to dto to the list
            organizationsDtoList.Add(responseOrganization);
        }
        return organizationsDtoList;
    }

    // Fetch the organization by organizationId from the route parameter
    public async Task<GetOrganizationResponseDTO?> GetOrganizationByIdAsync(int organizationId)
    {
        var organization = await _db.Organizations.FindAsync(organizationId);

        if (organization is null)
        {
            return null;
        }

        GetOrganizationResponseDTO responseOrganization = new GetOrganizationResponseDTO
        {
            OrganizationId = organization!.Id,
            Name = organization.Name,
        };
        return responseOrganization;
    }

    // Delete organization by organizationId from the route parameter
    public async Task<bool> DeleteOrganizationAsync(int organizationId)
    {
        // Fetch organization and delete it
        if (await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            var organization = await _db.Organizations
            .FindAsync(organizationId);

            if (organization!.Id == organizationId && organization is not null)
            {
                _db.Organizations.Remove(organization);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<GetClassInOrganizationResponseDTO?> GetClassInOrganizationAsync(int organizationId, int classId)
    {
        // Fetch the class tuple, where the column OrganizationId equals organizationId and
        // the column Id equals classId from the route parameter
        var selectedClass = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == classId && c.OrganizationId == organizationId);

        if (selectedClass is null)
        {
            return null;
        }

        // Return correctly formatted response
        GetClassInOrganizationResponseDTO responseClassInOrganization = new GetClassInOrganizationResponseDTO
        {
            ClassId = selectedClass!.Id,
            Name = selectedClass.Name,
            OrganizationId = selectedClass.OrganizationId,
        };
        return responseClassInOrganization;
    }

    public async Task<List<GetClassInOrganizationResponseDTO>> GetClassesInOrganizationAsync(int organizationId)
    {
        // Fetch all class tuples, where the column OrganizationId equals route parameter organizationId
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == organizationId).AsNoTracking().ToListAsync();

        // List of ClassInOrganizationDTOs
        List<GetClassInOrganizationResponseDTO> classesInOrganizationDtoList = new List<GetClassInOrganizationResponseDTO>();

        foreach (Class selectedClass in classes)
        {
            GetClassInOrganizationResponseDTO responseClassInOrganization = new GetClassInOrganizationResponseDTO
            {
                ClassId = selectedClass.Id,
                Name = selectedClass.Name,
                OrganizationId = selectedClass.OrganizationId,
            };
            // Add each class that has been converted to dto to the list
            classesInOrganizationDtoList.Add(responseClassInOrganization);
        }
        return classesInOrganizationDtoList;
    }

    public async Task<bool> DeleteClassInOrganizationAsync(int organizationId, int classId)
    {
        // Fetch the class with classId and organizationId matching Route parameters
        if (await _db.Classes.AnyAsync(c => c.Id == classId && c.OrganizationId == organizationId))
        {
            var selectedClass = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == classId && c.OrganizationId == organizationId);

            if (selectedClass!.Id == classId && selectedClass.OrganizationId == organizationId)
            {
                _db.Classes.Remove(selectedClass);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<CreateClassResponseDTO?> CreateClassInOrganizationAsync(CreateClassRequestDTO createClass, int organizationId)
    {
        // Fetch organization instance from db (Required in Class model)
        var organization = await _db.Organizations.FindAsync(organizationId);

        if (organization is null)
        {
            return null;
        }

        // Create new instance of class
        Class createdClass = new Class
        {
            Name = createClass.Name,
            OrganizationId = organization!.Id,
            Organization = organization,
        };

        // Check if the createdClass already exists as a tuple in the classes relation.
        // We check this by attributes: Name and organizationId
        // Ensures that an organization can not hold two classes named: 4a
        bool checkTupleAlreadyExist = await _db.Classes.AnyAsync(c => c.Name == createClass.Name && c.OrganizationId == organizationId);

        // Insert the new instance tuple into the db.
        if (createdClass.Name is not null && createdClass.Organization is not null && checkTupleAlreadyExist is false)
        {
            _db.Classes.Add(createdClass);
            await _db.SaveChangesAsync();
        }

        // Fetch the new class in the db. 
        // The db makes the class Id on insert, which is required in the response body
        // It is therefore we insert it in the db, and then fetches that same entry now containing id. 
        Class newClass = _db.Classes.FirstOrDefault(c => c.Name == createdClass.Name
                && c.OrganizationId == createdClass.OrganizationId)!;

        if (newClass is null)
        {
            return null;
        }

        // Return correctly formatted response
        CreateClassResponseDTO responseClass = new CreateClassResponseDTO
        {
            ClassId = newClass!.Id,
            Name = newClass.Name,
            OrganizationId = newClass.OrganizationId,
        };

        return responseClass;

    }


    public async Task<GetOrganizationResponseDTO?> CreateOrganizationAsync(CreateOrganizationRequestDTO createOrganization)
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
        GetOrganizationResponseDTO responseOrganization = new GetOrganizationResponseDTO
        {
            OrganizationId = newOrganization.Id,
            Name = newOrganization.Name,
        };

        return responseOrganization;

    }

    public async Task<GetOrganizationResponseDTO?> UpdateOrganizationAsync(CreateOrganizationRequestDTO updateOrganization, int organizationId)
    {
        // Find the organization in db
        var organization = await _db.Organizations.FindAsync(organizationId);

        if (organization is null)
        {
            return null;
        }

        // Check if the name specified in the request is already taken, including if the name is not changed it is taken. 
        bool nameAlreadyExist = await _db.Organizations.AnyAsync(o => o.Name == updateOrganization.Name);

        if (organization!.Id == organizationId && nameAlreadyExist is false && updateOrganization.Name is not null)
        {
            organization.Name = updateOrganization.Name;
        }

        // Save the updated organization in db. 
        await _db.SaveChangesAsync();

        // Find the updated organization in db
        var updatedOrganization = await _db.Organizations.FindAsync(organizationId);

        if (updatedOrganization is null)
        {
            return null;
        }

        // Return correctly formatted response
        GetOrganizationResponseDTO responseOrganization = new GetOrganizationResponseDTO
        {
            OrganizationId = updatedOrganization!.Id,
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

    public async Task<(int, UserRole?)> CreateOrUpdateUserRoleAsync(int organizationId, int nonCitizenId, RoleType role)
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