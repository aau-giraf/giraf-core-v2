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

    public async Task<Image?> CreateImagesAsync(PostImageDTO imageRequest)
    {
        var citizen = await _db.Users.FindAsync(imageRequest.CitizenId);

        StoreImage(imageRequest.File, imageRequest.Name);

        
        var image = new Image
        {
            Name = imageRequest.Name

        }
        
        _db.Images.Add(image);
        await _db.SaveChangesAsync();

        return await GetImageByIdAsync(image.Id);
    }
    // stores image on core machine
    private string StoreImage(IFormFile file, string name)
    {
        // gives a unique ID and keeps the extension
        string path = Path.Combine("images", $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}");

        using var stream = File.Create(path);

        file.CopyTo(stream);

        return path;
    }

}
