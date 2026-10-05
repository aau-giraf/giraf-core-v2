using System.Reflection.Metadata.Ecma335;
using System.Security.Principal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace giraf_core_v2.Services;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    // Fetch the organizations
    public async Task<List<Organization>> GetOrganizationsAsync()
    {   
        var organizations = await _db.Organizations.ToListAsync();;
        
        return organizations;
    }

    // Fetch the organization by org_id from route
    public async Task<Organization> GetOrganizationByIdAsync(int org_id)
    {   
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);
        
        return organization;
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

    public async Task<Class> GetClassInOrganizationAsync(int org_id, int class_id)
    {   
        
        // Fetch the classes tuples, where the column OrganizationId equals org_id and
        // find the class where the columns Id equals class_id from route
        var oneClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);

        return oneClass;
    }

    public async Task<List<Class>> GetClassesInOrganizationAsync(int org_id)
    {   
        // Fetch all class tuples, where the column OrganizationId equals org_id
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == org_id).AsNoTracking().ToListAsync();

        return classes;
    }

    public async Task<bool> DeleteClassInOrganizationAsync(int org_id, int class_id)
    {   
        // Fetch the specific class like before and delete it.
        var oneClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);
        
        if (oneClass.Id == class_id)
        {
            _db.Classes.Remove(oneClass);
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


        // Insert the new instance tuple into the db.
        if (createdClass.Name is not null && createdClass.Organization is not null) {
        _db.Classes.Add(createdClass);
        await _db.SaveChangesAsync();
        }

        // Fetch the new class in the DB. 
        // The DB makes the class Id on entry, which is required in the Response
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

}