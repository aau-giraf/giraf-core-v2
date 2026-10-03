using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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
    public async Task<Organization> GetOrganizationByIdAsync(Guid org_id)
    {   
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);
        
        return organization;
    }

    // Delete organization by org_id from route
    public async Task<bool> DeleteOrganizationAsync(Guid org_id)
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

    public async Task<Class> GetClassInOrganizationAsync(Guid org_id, Guid class_id)
    {   
        
        // Fetch the classes tuples, where the column OrganizationId equals org_id and
        // find the class where the columns Id equals class_id from route
        var oneClass = await _db.Classes
            .Where(c => c.OrganizationId == org_id).FirstOrDefaultAsync(c => c.Id == class_id);

        return oneClass;
    }

    public async Task<List<Class>> GetClassesInOrganizationAsync(Guid org_id)
    {   
        // Fetch all class tuples, where the column OrganizationId equals org_id
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == org_id).AsNoTracking().ToListAsync();

        return classes;
    }

    public async Task<bool> DeleteClassInOrganizationAsync(Guid org_id, Guid class_id)
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

    public async Task<Class> CreateClassInOrganizationAsync(string className, Guid org_id)
    {
        // Create new class in organization
        Class createdClass = new Class() {Id = Guid.NewGuid(), Name = className, OrganizationId = org_id};
        
        if (createdClass.Name is not null)
        {
            await _db.SaveChangesAsync();
        }
        return createdClass;
        
    }

}

