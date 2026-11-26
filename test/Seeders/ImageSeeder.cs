// Optional Image seeding parameters.
// ClassId & CitizenId: IDs == 0 -> random id, IDs == null -> null, IDs != 0 -> supplied int.
// OrganizationId:
//      CreateFaker(...): ID == 0 || null -> random id, else it will be the supplied int.
//      CreateFakerRequest(...): IDs == 0 -> random id, IDs == null -> null, IDs != 0 -> supplied int.
// FileExtension == null -> random not in BlockedFileExtensions, else FileExtension -> supplied extension
namespace giraf_core_v2.Tests.Seeders;

internal record ImageOptions(int? ClassId = 0, int? OrganizationId = 0, int? CitizenId = 0, string? FileExtension = null);

internal class ImageSeeder : Seeder<Image, ImageOptions>
{

    public override async Task<Image> SeedAsync(AppDbContext db, int seed, ImageOptions? options = null)
    {
        var image = Create(seed, options);

        db.Images.Add(image);
        await db.SaveChangesAsync();

        return image;
    }

    public override async Task<List<Image>> SeedManyAsync(AppDbContext db, int seed, int count, List<ImageOptions>? options = null)
    {
        var images = CreateMany(seed, count, options);

        db.Images.AddRange(images);
        await db.SaveChangesAsync();

        return images;
    }

    public override Faker<Image> CreateFaker(int seed, ImageOptions? options = null)
    {
        options ??= new(ClassId: null, OrganizationId: 0, CitizenId: null);

        return new Faker<Image>()
            .UseSeed(seed)
            .RuleFor(i => i.TitleEn, f => f.Lorem.Word())
            .RuleFor(i => i.CitizenId, f => options.CitizenId != 0 ? options.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.OrganizationId, f => options.OrganizationId != null && options.OrganizationId != 0 ? options.OrganizationId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.ClassId, f => options.ClassId != 0 ? options.ClassId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.Path, f => f.System.FileName())
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5))
            .RuleFor(i => i.TicketId, f => f.Lorem.Word())
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }

    public PostImageRequestDTO CreateRequestBody(int seed, ImageOptions? options = null)
    {
        return CreateFakerRequest(seed, options).Generate();
    }

    private Faker<PostImageRequestDTO> CreateFakerRequest(int seed, ImageOptions? options = null)
    {
        options ??= new(ClassId: null, OrganizationId: 0, CitizenId: null);

        return new Faker<PostImageRequestDTO>()
            .UseSeed(seed)
            .RuleFor(i => i.TitleEn, f => f.Lorem.Word())
            .RuleFor(i => i.OrganizationId,
                f => options.OrganizationId != 0 ? options.OrganizationId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.CitizenId,
                f => options.CitizenId != 0 ? options.CitizenId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.ClassId, f => options.ClassId != 0 ? options.ClassId : f.Random.Int(1, Int32.MaxValue))
            .RuleFor(i => i.File, f =>
            {
                var bytes = f.Random.Bytes(32);
                var stream = new MemoryStream(bytes);
                var chosen = options.FileExtension ?? RandomCommonFileExtension(f);
                return new FormFile(stream, 0, bytes.Length, "file", $"{f.Lorem.Word()}.{chosen}");
            });
    }

    private static readonly HashSet<string> BlockedFileExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        "jpeg", "jpg", "png", "heic", "webp"
    };

    private static string RandomCommonFileExtension(Faker f)
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