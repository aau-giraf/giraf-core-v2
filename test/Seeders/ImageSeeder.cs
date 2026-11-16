// Optional Image seeding parameters.
// IDs == 0 = random id, IDs == null = null, IDs != 0 = supplied int.
// Ext == null = random not in BlockedFileExtensions, else Ext = supplied extension

internal record ImageOptions(int? ClassId = 0, int? OrgId = 0, int? CitizenId = 0, string? Ext = null);

internal class ImageSeeder : Seeder<Image, ImageOptions>
{

    public override async Task<Image> SeedAsync(AppDbContext db, int seed, ImageOptions? opt = null)
    {
        var image = Create(seed, opt);

        db.Images.Add(image);
        await db.SaveChangesAsync();

        return image;
    }

    public override async Task<List<Image>> SeedManyAsync(AppDbContext db, int seed, int count, List<ImageOptions>? opt = null)
    {
        var images = CreateMany(seed, count, opt);

        db.Images.AddRange(images);
        await db.SaveChangesAsync();

        return images;
    }

    public override Faker<Image> CreateFaker(int seed, ImageOptions? opt = null)
    {
        if (opt == null)
        {
            // if no options were supplied, assign the image to a random citizen.
            opt = new(null, null, 0);
        }
        return new Faker<Image>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.CitizenId, f => opt.CitizenId != 0 ? opt.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.OrganizationId, f => opt.OrgId != 0 ? opt.OrgId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.ClassId, f => opt.ClassId != 0 ? opt.ClassId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.Path, f => f.System.FileName())
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }

    public PostImageDTO CreateRequestBody(int seed, ImageOptions? opt = null)
    {
        return CreateFakerRequest(seed, opt).Generate();
    }

    private Faker<PostImageDTO> CreateFakerRequest(int seed, ImageOptions? opt = null)
    {
        if (opt == null)
        {
            opt = new(null, null, 0);
        }
        return new Faker<PostImageDTO>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.OrganizationId, f => opt.OrgId != 0 ? opt.OrgId : f.Random.Int(1, 100))
            .RuleFor(i => i.CitizenId, f => opt.CitizenId != 0 ? opt.CitizenId : f.Random.Int(1, 100))
            .RuleFor(i => i.ClassId, f => opt.ClassId != 0 ? opt.ClassId : f.Random.Int(1, 100))
            .RuleFor(i => i.File, f =>
            {
                var bytes = f.Random.Bytes(32);
                var stream = new MemoryStream(bytes);
                var chosen = opt.Ext ?? RandomCommonFileExt(f);
                return new FormFile(stream, 0, bytes.Length, "file", $"{f.Lorem.Word()}.{chosen}");
            });
    }

    private static readonly HashSet<string> BlockedFileExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpeg", "jpg", "png", "heic", "webp"
    };

    private static string RandomCommonFileExt(Faker f)
    {
        string extension;
        do
        {
            extension = f.System.CommonFileExt();
        }
        while (BlockedFileExtensions.Contains(extension));

        return extension;
    }
}