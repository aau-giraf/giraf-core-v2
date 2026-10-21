using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.VisualBasic;

namespace giraf_core_v2.Services;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<ResponseGetOrganizationDTO>> GetOrganizationsAsync()
    {
        // Fetch organizations
        var organizations = await _db.Organizations.ToListAsync();

        // List of organizationDTOs
        List<ResponseGetOrganizationDTO> organizationsDtoList = new List<ResponseGetOrganizationDTO>();

        foreach (Organization organization in organizations)
        {
            ResponseGetOrganizationDTO responseOrganizationDTO = new ResponseGetOrganizationDTO
            {
                Id = organization.Id,
                Name = organization.Name,
            };
            // Add each organization that has been converted to dto to the list
            organizationsDtoList.Add(responseOrganizationDTO);
        }
        return organizationsDtoList;
    }

    // Fetch the organization by org_id from Route parameter
    public async Task<ResponseGetOrganizationDTO?> GetOrganizationByIdAsync(int org_id)
    {
        var organization = await _db.Organizations.FindAsync(org_id);

        if (organization is null)
        {
            return null;
        }

        ResponseGetOrganizationDTO responseOrganizationDTO = new ResponseGetOrganizationDTO
        {
            Id = organization!.Id,
            Name = organization.Name,
        };
        return responseOrganizationDTO;
    }

    // Delete organization by org_id from Route parameter
    public async Task<bool> DeleteOrganizationAsync(int org_id)
    {
        // Fetch organization and delete it
        if (await _db.Organizations.AnyAsync(o => o.Id == org_id))
        {
            var organization = await _db.Organizations
            .FindAsync(org_id);

            if (organization!.Id == org_id && organization is not null)
            {
                _db.Organizations.Remove(organization);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<ResponseGetClassInOrganizationDTO?> GetClassInOrganizationAsync(int org_id, int class_id)
    {
        // Fetch the class tuple, where the column OrganizationId equals org_id and
        // the column Id equals class_id from Route parameter
        var selectedClass = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == class_id && c.OrganizationId == org_id);

        if (selectedClass is null)
        {
            return null;
        }

        // Return correctly formatted response
        ResponseGetClassInOrganizationDTO responseClassInOrganizationDTO = new ResponseGetClassInOrganizationDTO
        {
            Id = selectedClass!.Id,
            Name = selectedClass.Name,
            OrganizationId = selectedClass.OrganizationId,
        };
        return responseClassInOrganizationDTO;
    }

    public async Task<List<ResponseGetClassInOrganizationDTO>> GetClassesInOrganizationAsync(int org_id)
    {
        // Fetch all class tuples, where the column OrganizationId equals Route parameter org_id
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == org_id).AsNoTracking().ToListAsync();

        // List of ClassInOrganizationDTOs
        List<ResponseGetClassInOrganizationDTO> classesInOrganizationDtoList = new List<ResponseGetClassInOrganizationDTO>();

        foreach (Class selectedClass in classes)
        {
            ResponseGetClassInOrganizationDTO responseClassInOrganizationDTO = new ResponseGetClassInOrganizationDTO
            {
                Id = selectedClass.Id,
                Name = selectedClass.Name,
                OrganizationId = selectedClass.OrganizationId,
            };
            // Add each class that has been converted to dto to the list
            classesInOrganizationDtoList.Add(responseClassInOrganizationDTO);
        }
        return classesInOrganizationDtoList;
    }

    public async Task<bool> DeleteClassInOrganizationAsync(int org_id, int class_id)
    {
        // Fetch the class with classId and organizationId matching Route parameters
        if (await _db.Classes.AnyAsync(c => c.Id == class_id && c.OrganizationId == org_id))
        {
            var selectedClass = await _db.Classes
            .FirstOrDefaultAsync(c => c.Id == class_id && c.OrganizationId == org_id);

            if (selectedClass!.Id == class_id && selectedClass.OrganizationId == org_id)
            {
                _db.Classes.Remove(selectedClass);
                await _db.SaveChangesAsync();
                return true;
            }
        }
        return false;
    }

    public async Task<ResponseCreateClassDTO?> CreateClassInOrganizationAsync(RequestCreateClassDTO createClass, int org_id)
    {
        // Fetch organization instance from db (Required in Class model)
        var organization = await _db.Organizations.FindAsync(org_id);

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
        bool checkTupleAlreadyExist = await _db.Classes.AnyAsync(c => c.Name == createClass.Name && c.OrganizationId == org_id);

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
        ResponseCreateClassDTO responseClass = new ResponseCreateClassDTO
        {
            Id = newClass!.Id,
            Name = newClass.Name,
            OrganizationId = newClass.OrganizationId,
        };

        return responseClass;

    }


    public async Task<ResponseGetOrganizationDTO?> CreateOrganizationAsync(RequestCreateOrganizationDTO createOrganization)
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

    public async Task<ResponseGetOrganizationDTO?> UpdateOrganizationAsync(RequestCreateOrganizationDTO updateOrganization, int org_id)
    {
        // Find the organization in db
        var organization = await _db.Organizations.FindAsync(org_id);

        if (organization is null)
        {
            return null;
        }

        // Check if the name specified in the request is already taken, including if the name is not changed it is taken. 
        bool nameAlreadyExist = await _db.Organizations.AnyAsync(o => o.Name == updateOrganization.Name);

        if (organization!.Id == org_id && nameAlreadyExist is false && updateOrganization.Name is not null)
        {
            organization.Name = updateOrganization.Name;
        }

        // Save the updated organization in db. 
        await _db.SaveChangesAsync();

        // Find the updated organization in db
        var updatedOrganization = await _db.Organizations.FindAsync(org_id);

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
            db.UserRoles.Remove(userRole);
            await db.SaveChangesAsync();
            //Deletion occured, indicate for HTTP response
            return true;
        }
        //Corresponding row didn't exist, no deletion occurred, indicate for HTTP response
        return false;
    }

    public async Task<ResponseCreateUserOrganizationDTO?> CreateUserInOrganizationAsync(int org_id, int user_id)
    {

        // Fetch organization instance from db (Required in UserOrganization model)
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);

        if (organization is null)
        {
            return null;
        }

        // Fetch non-citizen instance from db (Required in UserOrganization model)
        var user = await _db.NonCitizens.FirstOrDefaultAsync(u => u.Id == user_id);

        if (user is null)
        {
            return null;
        }

        // Create new instance of UserOrganization
        UserRole createdUserRole = new UserRole
        {
            NonCitizenId = user!.Id,
            NonCitizen = user,
            Role = RoleType.Parent, //Compiler Error Fix
            OrganizationId = organization!.Id,
            Organization = organization,
        };

        // Check if the userOrganization specified in Route parameter is already a tuple in the UserOrganization relation.
        bool checkUserOrganizationAlreadyExist = await _db.UserRoles.AnyAsync(ur => ur.NonCitizenId == user_id && ur.OrganizationId == org_id);

        // Insert the new UserOrganization tuple into the db.
        if (createdUserRole.NonCitizenId == user_id && createdUserRole.OrganizationId == org_id
                && checkUserOrganizationAlreadyExist is false
                && createdUserRole.Organization is not null
                && createdUserRole.NonCitizen is not null)
        {
            _db.UserRoles.Add(createdUserRole);
            await _db.SaveChangesAsync();
        }

        // Fetch the new UserOrganization
        var newUserOrganization = await _db.UserRoles.FirstAsync(ur => ur.NonCitizenId == user_id && ur.OrganizationId == org_id);

        if (newUserOrganization is null)
        {
            return null;
        }

        // Return correctly formatted response
        ResponseCreateUserOrganizationDTO responseUserOrganization = new ResponseCreateUserOrganizationDTO
        {
            UserId = newUserOrganization.NonCitizenId,
            OrganizationId = newUserOrganization.OrganizationId,
        };

        return responseUserOrganization;

    }

}