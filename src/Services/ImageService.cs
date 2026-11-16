using giraf_core_v2.DTOs.Images;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace giraf_core_v2.Services;

public class ImageService(AppDbContext db, IMemoryCache cache, HttpClient http)
{
    private readonly AppDbContext _db = db;
    
    // How long a rejected ticketId is saved
    private static readonly TimeSpan RejectedTicketLifetime = TimeSpan.FromMinutes(10);

    public async Task<Image?> GetImageByIdAsync(int image_id)
    {
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.Id == image_id);
        return image;
    }

    public async Task<List<Image>?> GetImagesByCitizenIdAsync(int cit_id)
    {
        var citizen = await _db.Citizens.FindAsync(cit_id);

        return citizen == null ? null : await _db.Images
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
            .FirstOrDefaultAsync(c => c.Id == imageRequest.ClassId);

            return class_?.Organization;
        }
        else
        {
            var citizen = await _db.Citizens
            .AsNoTracking()
            .Include(c => c.Class)
            .ThenInclude(c => c!.Organization)
            .FirstOrDefaultAsync(c => c.Id == imageRequest.CitizenId);

            return citizen?.Class?.Organization;
        }
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
            TitleEn = imageRequest.TitleEn,
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

    public async Task<string?> GenerateImage(PostImagePromptDTO postImagePromptDto, int userId)
    {
        var ticketId = Guid.NewGuid().ToString();

        var isCitizen = await _db.Citizens.AnyAsync(c => c.Id == userId);
        if (isCitizen)
        {
            // A citizen can only generate images for themselves
            postImagePromptDto.CitizenId = userId;
            postImagePromptDto.ClassId = null;
            postImagePromptDto.OrganizationId = null;
        }
        
        var request = new AiGenerationRequestDTO
        {
            TicketId = ticketId,
            UserId = userId,
            Prompt = postImagePromptDto.Prompt,
            CitizenId = postImagePromptDto.CitizenId,
            ClassId = postImagePromptDto.ClassId,
            OrganizationId = postImagePromptDto.OrganizationId,
        };

        try
        {
            // We need to get the generate endpoint from AI, where they should accept the AiGenerationRequestDTO
            var response = await http.PostAsJsonAsync("generate", request);
            return response.IsSuccessStatusCode ? ticketId : null;
        }
        catch (HttpRequestException) { return null; }  // AI backend is down
        catch (TaskCanceledException) { return null; }  // timeout
    }

    public void MarkInappropriate(string ticketId)
    {
        // Set the ticketId inappropriate to true, meaning the ticketId will not get an image generated
        cache.Set(ticketId, true, RejectedTicketLifetime);
    }

    public async Task<ReqIsImageDoneDTO> IsImageDone(string ticketId)
    {
        // lookup the image from the ticket id, if it returns null its not yet created
        var image = await _db.Images
            .AsNoTracking()
            .FirstOrDefaultAsync(i => i.TicketId == ticketId);

        if (image == null)
        {
            // lookup in the in-memory what the ticketId status is
            if (cache.TryGetValue(ticketId, out _))
            {
                return new ReqIsImageDoneDTO { Status = ImageStatus.Inappropriate, TicketId = ticketId };
            }
            // Return image status pending
            return new ReqIsImageDoneDTO { Status = ImageStatus.Pending, TicketId = ticketId };
        }

        var requestDTO = new ReqIsImageDoneDTO() { TicketId = ticketId, TitleEn = image.TitleEn, Status = ImageStatus.Done, ImageId = image.Id};

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