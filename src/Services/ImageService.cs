using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class ImageService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Image?> GetImageByIdAsync(int image_id)
    {
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == image_id);
        return image;
    }

    public async Task<List<Image>?> GetImagesByCitizenIdAsync(int cit_id)
    {
        var citizen = await _db.Citizens
        .AsNoTracking()
        .Include(c => c.Images)
        .FirstOrDefaultAsync();


        return citizen?.Images.ToList();
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
        if (removedimage is null) return false;

        _db.Images.Remove(removedimage);
        File.Delete(removedimage.Path);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<Organization?> FindOrganizationAsync(PostImageDTO imageRequest)
    {
        int orgId;
        if (imageRequest.CitizenId == null)
        {
            var class_ = await _db.Classes.FindAsync(imageRequest.ClassId);
            orgId = class_!.OrganizationId;
        }
        else
        {
            var citizen = await _db.Citizens.FindAsync(imageRequest.CitizenId);
            var class_ = await _db.Classes.FindAsync(citizen!.ClassId);
            orgId = class_!.OrganizationId;
        }
        return await _db.Organizations.FindAsync(orgId);
    }

    public async Task<Image?> CreateImagesAsync(PostImageDTO imageRequest)
    {
        var stringPath = await StoreImageAsync(imageRequest.File);

        var organization = imageRequest.OrganizationId != null ? await _db.Organizations.FindAsync(imageRequest.OrganizationId) : await FindOrganizationAsync(imageRequest);

        if (organization == null)
        { // If it is not possible to connect the request to an org return null
            return null;
        }

        var image = new Image
        {
            Name = imageRequest.Name,
            Path = stringPath,
            CitizenId = imageRequest.CitizenId,
            ClassId = imageRequest.ClassId,
            OrganizationId = organization!.Id,
        };

        _db.Images.Add(image);
        await _db.SaveChangesAsync();

        return await GetImageByIdAsync(image.Id);
    }
    // stores image on core machine
    private static async Task<string> StoreImageAsync(IFormFile file)
    {
        const string folder = "images";
        Directory.CreateDirectory(folder);

        // gives a unique ID and keeps the extension
        string path = Path.Combine("images", $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

        await using var stream = File.Create(path);
        await file.CopyToAsync(stream);

        return path;
    }
}