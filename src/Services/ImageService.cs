using Microsoft.EntityFrameworkCore;

public class ImageService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Image> GetIDInImagesAsync(int image_id)
    {
        var image = await _db.Images
            .FirstOrDefaultAsync(c=> c.Id == image_id);
        return image;
    }
}