using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class ImageService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Image?> GetImageByIdAsync(int imageId)
    {
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == imageId);
        return image;
    }

    public async Task<List<Image>?> GetImagesByCitizenIdAsync(int citizenId)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);

        return citizen == null ? null : await _db.Images
            .AsNoTracking()
            .Where(i => i.CitizenId == citizenId)
            .ToListAsync();
    }

    public async Task<List<Image>> GetAllImagesAsync()
    {
        return await _db.Images
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<bool> DeleteImageAsync(int imageId)
    {
        var removedImage = await _db.Images.FindAsync(imageId);
        if (removedImage is null) return false;

        _db.Images.Remove(removedImage);
        File.Delete(removedImage.Path);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<Organization?> FindOrganizationAsync(PostImageRequestDTO imageRequest)
    {
        if (imageRequest.CitizenId == null)
        {
            var schoolClass = await _db.Classes
            .AsNoTracking()
            .Include(c => c.Organization)
            .FirstOrDefaultAsync();

            return schoolClass?.Organization;
        }
        else
        {
            var citizen = await _db.Citizens
            .AsNoTracking()
            .Include(c => c.Class)
            .ThenInclude(c => c!.Organization)
            .FirstOrDefaultAsync();

            return citizen?.Class?.Organization;
        }
    }

    public async Task<Image?> CreateImagesAsync(PostImageRequestDTO imageRequest)
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