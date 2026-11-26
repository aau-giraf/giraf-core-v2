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

    private async Task<Organization?> FindOrganizationAsync(int? citizenId, int? classId)
    {
        if (citizenId == null)
        {
            var schoolClass = await _db.Classes
            .AsNoTracking()
            .Include(c => c.Organization)
            .FirstOrDefaultAsync(c => c.Id == classId);

            return schoolClass?.Organization;
        }
        else
        {
            var citizen = await _db.Citizens
            .AsNoTracking()
            .Include(c => c.Class)
            .ThenInclude(c => c!.Organization)
            .FirstOrDefaultAsync(c => c.Id == citizenId);

            return citizen?.Class?.Organization;
        }
    }
    
    public async Task<Image?> CreateGeneratedImagesAsync(PostGeneratedImageRequestDTO imageRequest)
    {
        var stringPath = await StoreImageAsync(imageRequest.File);

        var organization = imageRequest.OrganizationId != null ? await _db.Organizations.FindAsync(imageRequest.OrganizationId) : await FindOrganizationAsync(imageRequest.CitizenId, imageRequest.ClassId);
        if (organization == null)
        { // If it is not possible to connect the request to an org return null
            return null;
        }

        var image = new Image
        {
            TitleEn = imageRequest.TitleEn,
            TitleDa = imageRequest.TitleDa,
            Path = stringPath,
            CitizenId = imageRequest.CitizenId,
            ClassId = imageRequest.ClassId,
            OrganizationId = organization!.Id,
            TicketId = imageRequest.TicketId
        };

        _db.Images.Add(image);
        await _db.SaveChangesAsync();

        return await GetImageByIdAsync(image.Id);
    }

    public async Task<Image?> CreateImagesAsync(PostImageRequestDTO imageRequest)
    {
        var stringPath = await StoreImageAsync(imageRequest.File);

        var organization = imageRequest.OrganizationId != null ? await _db.Organizations.FindAsync(imageRequest.OrganizationId) : await FindOrganizationAsync(imageRequest.CitizenId, imageRequest.ClassId);
        if (organization == null)
        { // If it is not possible to connect the request to an org return null
            return null;
        }

        var image = new Image
        {
            TitleEn = imageRequest.TitleEn,
            TitleDa = imageRequest.TitleDa,
            Path = stringPath,
            CitizenId = imageRequest.CitizenId,
            ClassId = imageRequest.ClassId,
            OrganizationId = organization!.Id,
        };

        _db.Images.Add(image);
        await _db.SaveChangesAsync();

        return await GetImageByIdAsync(image.Id);
    }

    // Service for foodplanner/vta to call after they have generated an image using the AI-backend
    public async Task<IsImageDoneResponseDTO> IsImageDone(string ticketId)
    {
        // Lookup the image in the db using the TicketId 
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.TicketId == ticketId);

        // If the lookup returns null its not yet created
        if (image == null)
        {
            // Return image status pending
            return new IsImageDoneResponseDTO { Status = ImageStatus.Pending, TicketId = ticketId };
        }

        // Else return the DTO with the given instance that VTA/Foodplanner can use
        var requestDTO = new IsImageDoneResponseDTO() { TicketId = ticketId, TitleEn = image.TitleEn, TitleDa = image.TitleDa, Status = ImageStatus.Done, ImageId = image.Id };

        return requestDTO;
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