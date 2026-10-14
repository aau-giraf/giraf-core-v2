public static class ImageSeeder
{
    public record SeededImage(Image Image);

    public static async Task<SeededImage> SeedAsync(AppDbContext db, int seed)
    {
        var image = Create(seed);

        db.Images.Add(image.Image);
        await db.SaveChangesAsync();

        return image;
    }

    public static SeededImage Create(int seed)
    {
        var faker = new Faker<string>().UseSeed(seed);

        var image = CreateFaker(seed);

        return new SeededImage(image);
    }

    public static Faker<Image> CreateFaker(int seed)
    {
        return new Faker<Image>()
        .UseSeed(seed)
        .RuleFor(i => i.Name, f => f.Lorem.Word())
        .RuleFor(i => i.CitizenId, f => f.Random.Int(1, 100))
        .RuleFor(i => i.Path, f => f.System.FilePath())
        .RuleFor(i => i.OrganizationId, f => f.Random.Int(1, 100))
        .RuleFor(i => i.StorageKey, f => f.Random.String())
        .RuleFor(i => i.FileName, f => f.System.FileName())
        .RuleFor(i => i.FileType, f => f.System.FileType())
        .RuleFor(i => i.SizeBytes, f => f.Random.Byte())
        .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }
}
