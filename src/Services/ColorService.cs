using Microsoft.EntityFrameworkCore;

public class ColorService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<Color>> GetColorsAsync()
    {
        return await _db.Colors.AsNoTracking().ToListAsync();
    }
}