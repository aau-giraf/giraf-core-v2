using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<ServiceResult<List<ClassDTO>>> GetClassesInOrganizationAsync(Organization organization)
    {   
        // Fetch orginization
        var foundOrganization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == organization.Id);

        if (organization is null) {
            return ServiceResult<List<ClassDTO>>.Fail(
            new ServiceError(ServiceErrorKind.NotFound, "Organization not found."));
        }
        
        var classes = await _db.Classes
            .Where(o => o.Id == organization.Id).AsNoTracking().ToListAsync();
        
        var dtos = classes.Select(c => c.ToDTO()).ToList();
        return ServiceResult<List<ClassDTO>>.Success(dtos);
        
    }

    public async Task<ServiceResult<OrganizationDTO>> GetOrganizationById(int id)
    {   
      var organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == id);

      if (organization is null) {
            return ServiceResult<OrganizationDTO>.Fail(
            new ServiceError(ServiceErrorKind.NotFound, "Organization not found."));
        }
        return ServiceResult<OrganizationDTO>.Success(organization.ToDTO());
    }

}

