// Optional Image seeding parameters.
// IDs == 0 = random id, IDs == null = null, IDs != 0 = supplied int.
// Ext == null = random not in BlockedFileExtensions, else Ext = supplied extension
internal record ImageOptions(int? ClassId = 0, int? OrgId = 0, int? CitizenId = 0, string? Ext = null);

internal class ImageSeeder : Seeder<Image, ImageOptions>
{

    public override async Task<Image> SeedAsync(AppDbContext db, int seed)
    {
        var image = Create(seed);

        db.Images.Add(image);
        await db.SaveChangesAsync();

        return image;
    }

    //TODO: merge with seedmany async
    public async Task<List<Image>> SeededManyForCitizenImagesAsync(AppDbContext db, int seed, int count, int? citizenId)
    {
        var faker = CreateFaker(seed)
            .RuleFor(i => i.CitizenId, _ => citizenId); // Overwrites the

        var images = faker.Generate(count)
            .ToList();

        db.Images.AddRange(images);
        await db.SaveChangesAsync();

        return images;
    }

    public PostImageDTO CreateRequestBody(int seed, int? organizationId = 0, int? citizenId = 0, int? classId = 0, string ext = "")
    {
        return CreateFakerRequest(seed, organizationId, citizenId, classId, ext).Generate();
    }

    public override async Task<List<Image>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var images = CreateManyFakers(seed, count);

        db.Images.AddRange(images);
        await db.SaveChangesAsync();

        return images;
    }

    public override Faker<Image> CreateFaker(int seed, ImageOptions? opt = null)
    {
        return new Faker<Image>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.CitizenId, f => f.Random.Int(1, 100))
            .RuleFor(i => i.Path, f => f.System.FileName())
            .RuleFor(i => i.OrganizationId, f => f.Random.Int(1, 100))
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }

    private Faker<PostImageDTO> CreateFakerRequest(int seed, int? organizationId = 0, int? citizenId = 0, int? classId = 0, string ext = "")
    {
        return new Faker<PostImageDTO>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.OrganizationId, f => organizationId != 0 ? organizationId : f.Random.Int(1, 100))
            .RuleFor(i => i.CitizenId, f => citizenId != 0 ? citizenId : f.Random.Int(1, 100))
            .RuleFor(i => i.ClassId, f => classId != 0 ? classId : f.Random.Int(1, 100))
            .RuleFor(i => i.File, f =>
            {
                var bytes = f.Random.Bytes(32);
                var stream = new MemoryStream(bytes);
                var chosen = ext != null ? ext : RandomCommonFileExt(f);
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