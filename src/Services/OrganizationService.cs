using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<Class>> GetClassesInOrganizationAsync(int org_id)
    {   
        
        // Fetch all class tuples, where the column OrganizationId equals org_id
        var classes = await _db.Classes
            .Where(c => c.OrganizationId == org_id).AsNoTracking().ToListAsync();

        return classes;
    }


    // Fetch the organization by org_id from request header
    public async Task<Organization> GetOrganizationByIdAsync(int org_id)
    {   
        var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);
        
        return organization;
    }

}

