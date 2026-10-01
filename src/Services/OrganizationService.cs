using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<Class>> GetClassesInOrganizationAsync(int org_id)
    {   
        // Fetch the organization first, here we use the org_id from the request header   
        var foundOrganization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);

        if (foundOrganization is null) {
            // Throw error
        }
        
        // Fetch all classes tuples, where the columns org_id corresponding to the one we got from the request header
        var classes = await _db.Classes
            .Where(o => o.Id == foundOrganization.Id).AsNoTracking().ToListAsync();
        
          if (classes is null) {
            // Throw error
        }

        return classes;
    }


    // Fetch the organization by org_id in request header
    public async Task<Organization> GetOrganizationByIdAsync(int org_id)
    {   
      var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == org_id);

      if (organization is null) {
            // Throw error
        }
    
        return organization;
    }

}

