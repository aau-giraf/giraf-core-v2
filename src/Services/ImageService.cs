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
        var citizenExists = await _db.Citizens.AnyAsync(c => c.UserId == cit_id);
        if (!citizenExists)return null;


        return await _db.Images
            .AsNoTracking()
            .Where(i => i.CitizenId == cit_id)
            .ToListAsync();
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
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<int> FindOrganizationAsync(PostImageDTO imageRequest)
    {
        if (imageRequest.CitizenId == null)
        {
            var class_ = await _db.Classes.FindAsync(imageRequest.ClassId);
            return class_!.OrganizationId;
        }
        else
        {
            var citizen = await _db.Citizens.FindAsync(imageRequest.CitizenId);
            var class_ = await _db.Classes.FindAsync(citizen!.ClassId);
            return class_!.OrganizationId;
        }
    }

    public async Task<Image?> CreateImagesAsync(PostImageDTO imageRequest)
    {
        var stringPath = await StoreImageAsync(imageRequest.File);

        var organizationId = imageRequest.OrganizationId ?? await FindOrganizationAsync(imageRequest);

        var image = new Image
        {
            Name = imageRequest.Name,
            Path = stringPath,
            CitizenId = imageRequest.CitizenId,
            ClassId = imageRequest.ClassId,
            OrganizationId = organizationId,
        };

        _db.Images.Add(image);
        await _db.SaveChangesAsync();

        return await GetImageByIdAsync(image.Id);
    }
    // stores image on core machine
    private static async Task<string> StoreImageAsync(IFormFile file)
    {
        // gives a unique ID and keeps the extension
        string path = Path.Combine("images", $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

        using var stream = File.Create(path);

        await file.CopyToAsync(stream);

        return path;
    }

}
