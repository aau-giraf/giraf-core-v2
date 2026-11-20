// Optional Image seeding parameters.
// ClassId & CitizenId: IDs == 0 -> random id, IDs == null -> null, IDs != 0 -> supplied int.
// OrgId:
//      CreateFaker(...): ID == 0 || null -> random id, else it will be the supplied int.
//      CreateFakerRequest(...): IDs == 0 -> random id, IDs == null -> null, IDs != 0 -> supplied int.
// Ext == null -> random not in BlockedFileExtensions, else Ext -> supplied extension
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
        var options = opt ?? new(ClassId: null, OrgId: 0, CitizenId: null);

        return new Faker<Image>()
            .UseSeed(seed)
            .RuleFor(i => i.TitleEn, f => f.Lorem.Word())
            .RuleFor(i => i.CitizenId, f => options.CitizenId != 0 ? options.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.OrganizationId, f => options.OrgId != null && options.OrgId != 0 ? options.OrgId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.ClassId, f => options.ClassId != 0 ? options.ClassId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.Path, f => f.System.FileName())
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5))
            .RuleFor(i => i.TicketId, f => f.Lorem.Word())
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }

    public PostImageDTO CreateRequestBody(int seed, ImageOptions? opt = null)
    {
        return CreateFakerRequest(seed, opt).Generate();
    }

    private Faker<PostImageDTO> CreateFakerRequest(int seed, ImageOptions? opt = null)
    {
        var options = opt ?? new(ClassId: null, OrgId: 0, CitizenId: null);

        return new Faker<PostImageDTO>()
            .UseSeed(seed)
            .RuleFor(i => i.TitleEn, f => f.Lorem.Word())
            .RuleFor(i => i.OrganizationId, f => options.OrgId != 0 ? options.OrgId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.CitizenId, f => options.CitizenId != 0 ? options.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.ClassId, f => options.ClassId != 0 ? options.ClassId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.File, f =>
            {
                var bytes = f.Random.Bytes(32);
                var stream = new MemoryStream(bytes);
                var chosen = options.Ext ?? RandomCommonFileExt(f);
                return new FormFile(stream, 0, bytes.Length, "file", $"{f.Lorem.Word()}.{chosen}");
            })
            .RuleFor(i => i.TicketId, f => f.Lorem.Word());
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