using Microsoft.EntityFrameworkCore;
namespace giraf_core_v2.Services;

public class ImageService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Image?> GetIDInImagesAsync(int image_id)
    {
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(c=> c.Id == image_id);
        return image;
    }

    public async Task<List<Image>> GetAllImagesAsync()
    {
        return await _db.Images
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> DeleteImageAsync(int image_id)
    {
        var removedimage = await _db.Images.FindAsync(image_id);
        if (removedimage is null)
        {
            return false;
        }
        _db.Images.Remove(removedimage);
        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Image?> CreateImagesAsync(CreateImageRequest request)
    {
        var organization = await _db.Organizations.FindAsync(request.OrganizationId);
        if (organization == null)
        {
            return null;
        }

        var image = new Image
        {
            name = request.Name,
            path = request.StorageKey,
            CitizenId = request.CitizenId,
            OrganizationId = organization.Id,
            Organization = organization,
            StorageKey = request.StorageKey,
            FileName = request.FileName,
            FileType = request.FileType,
            SizeBytes = request.SizeBytes
        };
        
        _db.Images.Add(image);
        await _db.SaveChangesAsync();
        
        return await GetIDInImagesAsync(image.Id);
    }
    
}


