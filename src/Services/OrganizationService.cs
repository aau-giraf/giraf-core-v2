using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace giraf_core_v2.Services;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // Fetch the organizations
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

    // Fetch the organization by org_id from route
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

    // Delete organization by org_id from route
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
        // Fetch the classes tuples, where the column OrganizationId equals org_id and
        // find the class where the columns Id equals class_id from route
        var selectedClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);
        
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
        // Fetch all class tuples, where the column OrganizationId equals org_id
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
        // Fetch the specific class like before and delete it.
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

    public async Task<ResponseCreateClassDTO> CreateClassInOrganizationAsync(RequestCreateClassDTO createClass)
    {
       
       // Fetch organization instance from db (Required in Class model)
       var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == createClass.OrganizationId);

       // Create new instance of class
       Class createdClass = new Class
		{
			Name = createClass.Name,
            OrganizationId = createClass.OrganizationId,
            Organization = organization,
		};

        // Check if the name specified by the user is already taken
        bool checkNameAlreadyExist = _db.Classes.Any(o => o.Name  == createClass.Name);

        // Insert the new instance tuple into the db.
        if (createdClass.Name is not null && createdClass.Organization is not null && checkNameAlreadyExist is false) {
        _db.Classes.Add(createdClass);
        await _db.SaveChangesAsync();
        }

        // Fetch the new class in the DB. 
        // The DB makes the class Id on insert, which is required in the Response
        // It is therefore we insert it in the DB, and then fetches that same entry now containing id. 
        Class newClass = _db.Classes.FirstOrDefault(c => c.Name == createdClass.Name && c.OrganizationId == createClass.OrganizationId);
        

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
       // Check if the name specified by the user is already taken
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
        ResponseGetOrganizationDTO responseClass = new ResponseGetOrganizationDTO
		{
			Id = newOrganization.Id,
            Name = newOrganization.Name,
		};

        return responseClass;
    
    }

}