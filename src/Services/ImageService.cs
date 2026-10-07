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

    public async Task<List<Image>> GetAllImagesAsync()
    {
        return await _db.Images.ToListAsync();
    }

    public async Task<bool> DeleteImageAsync(int image_id)
    {
        var removedimage = await _db.Images.FindAsync(image_id);
        if (removedimage.Id == image_id)
        {
            _db.Images.Remove(removedimage);
            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }


    /*public async Task<List<Image>> CreateImagesAsync()
    {
        var image = new Image();
        _db.Images.Add(image);
        await _db.SaveChangesAsync();
        return image;
    }
    */
}


