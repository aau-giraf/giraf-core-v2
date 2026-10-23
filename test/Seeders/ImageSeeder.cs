using Microsoft.AspNetCore.Http;


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

    public static async Task<Citizen> SeedCitizenForImageAsync(AppDbContext db, int seed)
    {
        var user = (await UserSeeder.SeedAsync(db, seed)).User;
        var guardian = (await UserSeeder.SeedAsync(db, seed + 1)).User;
        var girafClass = await ClassSeeder.SeedAsync(db, seed);

        var citizen = new Citizen
        {
            User = user,
            Guardian = guardian,
            Class = girafClass,
        };

        db.Citizens.Add(citizen);
        await db.SaveChangesAsync();

        return citizen;
    }

    public static async Task<SeededImage> SeededForCitizenImageAsync(AppDbContext db, int seed, int? citizenId)
    {
        var image = CreateFaker(seed)
            .RuleFor(i => i.CitizenId, _ => citizenId)
            .Generate();

        db.Images.Add(image);
        await db.SaveChangesAsync();

        return new SeededImage(image);
    }

    public static async Task<List<SeededImage>> SeededManyForCitizenImagesAsync(AppDbContext db, int seed, int count, int? citizenId)
    {
        var faker = CreateFaker(seed)
            .RuleFor(i => i.CitizenId, _ => citizenId);

        var images = faker.Generate(count)
            .Select(i => new SeededImage(i))
            .ToList();

        db.Images.AddRange(images.Select(x => x.Image));
        await db.SaveChangesAsync();

        return images;
    }

    private static SeededImage Create(int seed)
    {
        var image = CreateFaker(seed);

        return new SeededImage(image);
    }

    public static PostImageDTO CreateRequestBody(int seed)
    {
        return CreateFakerRequest(seed).Generate();
    }
    public static async Task<List<SeededImage>> SeedManyAsync(AppDbContext db, int seed, int count)
    {
        var images = CreateManyFakers(seed, count);

        db.Images.AddRange(images.Select(x => x.Image));
        await db.SaveChangesAsync();

        return images;
    }

    private static Faker<Image> CreateFaker(int seed)
    {
        return new Faker<Image>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.CitizenId, f => f.Random.Int(1, 100))
            .RuleFor(i => i.Path, f => f.System.FilePath())
            .RuleFor(i => i.OrganizationId, f => f.Random.Int(1, 100))
            .RuleFor(i => i.CreatedAt, f => f.Date.Past(5));
    }

    private static Faker<PostImageDTO> CreateFakerRequest(int seed)
    {
        return new Faker<PostImageDTO>()
            .UseSeed(seed)
            .RuleFor(i => i.Name, f => f.Lorem.Word())
            .RuleFor(i => i.OrganizationId, f => f.Random.Int(1, 100))
            .RuleFor(i => i.CitizenId, _ => null)
            .RuleFor(i => i.ClassId, _ => null)
            .RuleFor(i => i.File, f =>
            {
                var bytes = f.Random.Bytes(32);
                var stream = new MemoryStream(bytes);
                return new FormFile(stream, 0, bytes.Length, "file", $"{f.Lorem.Word()}.png");
            });
    }
    private static List<SeededImage> CreateManyFakers(int seed, int count)
    {
        var ImageFaker = CreateFaker(seed);

        var images = new List<SeededImage>();

        for (int i = 0; i < count; i++)
        {
            var image = ImageFaker.Generate();

            images.Add(new SeededImage(image));
        }

        return images;
    }
}