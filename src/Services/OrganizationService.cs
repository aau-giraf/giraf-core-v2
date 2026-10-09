using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    public async Task<ResponseGetOrganizationDTO> GetOrganizationByIdAsync(int org_id)
    {   
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);
        
        ResponseGetOrganizationDTO responseOrganizationDTO = new ResponseGetOrganizationDTO
		    {
			    Id = organization.Id,
                Name = organization.Name,
		    };

        return responseOrganizationDTO;
    }

    // Delete organization by org_id from Route parameter
    public async Task<bool> DeleteOrganizationAsync(int org_id)
    {   
        
        // Fetch organization and delete it
        var organization = await _db.Organizations
            .FirstOrDefaultAsync(o => o.Id == org_id);
        
        if (organization.Id == org_id)
        {
            _db.Organizations.Remove(organization);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<ResponseGetClassInOrganizationDTO> GetClassInOrganizationAsync(int org_id, int class_id)
    {   
        // Fetch the class tuple, where the column OrganizationId equals org_id and
        // the columns Id equals class_id from Route parameter
        var selectedClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);
        
        // Return correctly formatted response
        ResponseGetClassInOrganizationDTO responseOrganizationDTO = new ResponseGetClassInOrganizationDTO
		    {
			    Id = selectedClass.Id,
                Name = selectedClass.Name,
                OrganizationId = selectedClass.OrganizationId,
		    };

        return responseOrganizationDTO;
    }

    public async Task<List<ResponseGetClassInOrganizationDTO>> GetClassesInOrganizationAsync(int org_id)
    {   
        // Fetch all class tuples, where the column OrganizationId equals Route parameter org_id
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == org_id).AsNoTracking().ToListAsync();

        // List of ClassDTOs
        List<ResponseGetClassInOrganizationDTO> classesDtoList = new List<ResponseGetClassInOrganizationDTO>();  
        
        foreach (Class selectedClass in classes)
        { 
            ResponseGetClassInOrganizationDTO responseClassDTO = new ResponseGetClassInOrganizationDTO
		    {
			    Id = selectedClass.Id,
                Name = selectedClass.Name,
                OrganizationId = selectedClass.OrganizationId,
		    };
            // Add each class that has been converted to dto to the list
            classesDtoList.Add(responseClassDTO);
        } 
        return classesDtoList;
    }

    public async Task<bool> DeleteClassInOrganizationAsync(int org_id, int class_id)
    {   
        // Fetch the class with classId and organizationId matching Route parameters
        var selectedClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);
        
        if (selectedClass.Id == class_id)
        {
            _db.Classes.Remove(selectedClass);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<ResponseCreateClassDTO> CreateClassInOrganizationAsync(RequestCreateClassDTO createClass, int org_id)
    {
       // Fetch organization instance from db (Required in Class model)
       var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);

       // Create new instance of class
       Class createdClass = new Class
		{
			Name = createClass.Name,
            OrganizationId = org_id,
            Organization = organization,
		};

        // Check if the createdClass already exists as a tuple in the classes relation.
        // We check this by attributes: Name and organizationId
        // Ensures that an organization can not hold two classes named: 4a
        bool checkTupleAlreadyExist = _db.Classes.Any(c => c.Name  == createClass.Name && c.OrganizationId == org_id);

        // Insert the new instance tuple into the db.
        if (createdClass.Name is not null && createdClass.Organization is not null && checkTupleAlreadyExist is false) {
        _db.Classes.Add(createdClass);
        await _db.SaveChangesAsync();
        }

        // Fetch the new class in the DB. 
        // The DB makes the class Id on insert, which is required in the Response
        // It is therefore we insert it in the DB, and then fetches that same entry now containing id. 
        Class newClass = _db.Classes.FirstOrDefault(c => c.Name == createdClass.Name && c.Id == createdClass.Id);
        

        // Return correctly formatted response
        ResponseCreateClassDTO responseClass = new ResponseCreateClassDTO
		{
			Id = newClass.Id,
            Name = newClass.Name,
            OrganizationId = newClass.OrganizationId,
		};

        return responseClass;
    
    }


    public async Task<ResponseGetOrganizationDTO> CreateOrganizationAsync(RequestCreateOrganizationDTO createOrganization)
    { 
       // Check if the name specified in the requestbody is already taken
       bool checkNameAlreadyExist = _db.Organizations.Any(o => o.Name  == createOrganization.Name);

       // Create new instance of Organization
       Organization createdOrganization = new Organization
		{
			Name = createOrganization.Name,
		};

        // Insert the new instance tuple into the db.
        if (createdOrganization.Name is not null && checkNameAlreadyExist is false) {
        _db.Organizations.Add(createdOrganization);
        await _db.SaveChangesAsync();
        }

        // Fetch the new organization in the DB. 
        // The DB makes the organization Id on insert, which is required in the Response
        // It is therefore we insert it in the DB, and then fetches that same entry now containing id. 
        Organization newOrganization = _db.Organizations.FirstOrDefault(c => c.Name == createdOrganization.Name);
        
        // Return correctly formatted response
        ResponseGetOrganizationDTO responseOrganization = new ResponseGetOrganizationDTO
		{
			Id = newOrganization.Id,
            Name = newOrganization.Name,
		};

        return responseOrganization;
    
    }

    public async Task<ResponseGetOrganizationDTO> UpdateOrganizationAsync(RequestCreateOrganizationDTO createOrganization, int org_id)
    {
        // Find the organization in db
        var organization = await _db.Organizations.FindAsync(org_id);

        if (organization is null)
        {
            return null;
        }

        // Check if the name specified in the request is already taken, including if the name is not changed it is taken. 
        bool checkNameAlreadyExist = _db.Organizations.Any(o => o.Name  == organization.Name);
        
        if (organization.Name is not null)
        {
            organization.Name = createOrganization.Name;
        }

        // Save the updated organization in DB. 
        await _db.SaveChangesAsync();

        // Return correctly formatted response
        ResponseGetOrganizationDTO responseOrganization = new ResponseGetOrganizationDTO
		{
			Id = org_id,
            Name = organization.Name,
		};
        
        return responseOrganization;
    }

       public async Task<bool> DeleteUserInOrganizationAsync(int org_id, int user_id)
    {   
        // Fetch the userOrganization with userId and organizationId matching Route parameters
        var selectedUserOrganization = await _db.UserOrganizations
            .Where(uo => uo.OrganizationId == org_id ).FirstOrDefaultAsync(uo => uo.UserId == user_id);
        
        if (selectedUserOrganization.UserId == user_id && selectedUserOrganization.OrganizationId == org_id)
        {
            _db.UserOrganizations.Remove(selectedUserOrganization);
            await _db.SaveChangesAsync();
            return true;
        }
        return false;
    }

    public async Task<ResponseCreateUserOrganizationDTO> CreateUserInOrganizationAsync(int org_id, int user_id)
    {   
        
        // Fetch organization instance from db (Required in UserOrganization model)
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);

        // Fetch user instance from db (Required in UserOrganization model)
       var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == user_id);


       // Create new instance of UserOrganization
       UserOrganization createdUserOrganization = new UserOrganization
		{
			UserId = user_id,
            User = user,
            OrganizationId = org_id,
            Organization = organization,
		};

        // Check if the user_id specified in Route parameter is already a tuple in the UserOrganization relation.
        bool checkUserOrganizationAlreadyExist = _db.UserOrganizations.Any(uo => uo.UserId == user_id && uo.OrganizationId == org_id);

        // Insert the new instance tuple into the db.
        if (createdUserOrganization.Organization is not null && checkUserOrganizationAlreadyExist is false) {
        _db.UserOrganizations.Add(createdUserOrganization);
        await _db.SaveChangesAsync();
        } 

        // Return correctly formatted response
        ResponseCreateUserOrganizationDTO responseUserOrganization = new ResponseCreateUserOrganizationDTO
		{
			UserId = createdUserOrganization.UserId,
            OrganizationId = createdUserOrganization.OrganizationId,
		};

        return responseUserOrganization;

    }

}