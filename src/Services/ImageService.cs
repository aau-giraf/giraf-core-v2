using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Services;

public class ImageService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    private List<ImageResponseDTO>? ConvertImagesToDTO(List<Image> images)
    {
        if (images is null)
        {
            return null;
        }

        List<ImageResponseDTO> imagesDTO = [];
        foreach (Image image in images)
        {
            imagesDTO.Add(new ImageResponseDTO
            {
                ImageId = image.Id,
                Title = image.Name,
                File = Results.File(Path.GetFullPath(Path.Combine("images", image.Name)))
            });
        }

        return imagesDTO;
    }

    public async Task<List<ImageResponseDTO>?> GetImagesByCitizenIdAsync(int cit_id)
    {
        var citizen = await _db.Citizens.FindAsync(cit_id);

        if (citizen is null)
        {
            return null;
        }

        List<Image> images = await _db.Images.AsNoTracking()
                                             .Where(i => i.CitizenId == cit_id)
                                             .ToListAsync();

        return ConvertImagesToDTO(images);
    }

    public async Task<List<ImageResponseDTO>?> GetImagesByClassIdAsync(int classId)
    {
        var class_ = await _db.Classes.FindAsync(classId);
        if (class_ is null)
        {
            return null;
        }

        List<Image> images = await _db.Images.AsNoTracking().Where(i => i.ClassId == classId).ToListAsync();

        return ConvertImagesToDTO(images);
    }

    public async Task<List<ImageResponseDTO>?> GetImagesByOrganizationIdAsync(int organizationId)
    {
        var organization = await _db.Organizations.FindAsync(organizationId);
        if (organization is null)
        {
            return null;
        }

        List<Image> images = await _db.Images.AsNoTracking().Where(i => i.OrganizationId == organizationId).ToListAsync();

        return ConvertImagesToDTO(images);
    }

    public async Task<bool> DeleteImageAsync(int image_id)
    {
        var removedImage = await _db.Images.FindAsync(image_id);
        if (removedImage is null) return false;

        _db.Images.Remove(removedImage);
        File.Delete(removedImage.Path);
        await _db.SaveChangesAsync();
        return true;
    }

    private async Task<Organization?> FindOrganizationAsync(PostImageDTO imageRequest)
    {
        if (imageRequest.CitizenId == null)
        {
            var class_ = await _db.Classes
            .AsNoTracking()
            .Include(c => c.Organization)
            .FirstOrDefaultAsync();

            return class_?.Organization;
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

    public async Task<CreateImageResponseDTO?> CreateImagesAsync(PostImageDTO imageRequest)
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

        image = await _db.Images.FindAsync(image.Id);

        return image is null ? null : new CreateImageResponseDTO
        {
            ImageId = image.Id,
            Title = image.Name,
            File = Results.File(Path.GetFullPath(Path.Combine("images", image.Name))),
            OrganizationId = image.OrganizationId,
            CitizenId = image.CitizenId ?? null,
            ClassId = image.ClassId ?? null,
            Path = image.Path,
            CreatedAt = image.CreatedAt
        };
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