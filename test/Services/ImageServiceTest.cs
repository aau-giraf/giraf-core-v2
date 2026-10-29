using Xunit.Abstractions;

public class ImageServiceTest : TestBase
{
    public ImageServiceTest(ITestOutputHelper output) : base(output) { }

    [Fact]
    public async Task GetAllImagesAsync_ReturnsAllImages()
    {
        await using var db = CreateDbContext();
        var seededImages = await ImageSeeder.SeedManyAsync(db, Seed, 3);
        var expectedIds = seededImages.Select(s => s.Image.Id).OrderBy(id => id).ToList();

        var service = new ImageService(db);

        var result = await service.GetAllImagesAsync();

        Assert.Equal(3, result.Count);
        Assert.Equal(expectedIds, result.Select(i => i.Id).OrderBy(id => id).ToList());
    }

    [Fact]
    public async Task GetAllImagesAsync_ReturnsEmptyList_WhenNoImages()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);

        var result = await service.GetAllImagesAsync();

        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetIDInImagesAsync_ReturnsImage_WhenImageExists()
    {
        await using var db = CreateDbContext();

        var seededImage = await ImageSeeder.SeedAsync(db, Seed);

        var image = seededImage.Image;

        var service = new ImageService(db);

        var imageById = await service.GetImageByIdAsync(image.Id);

        Assert.Equal(image.Id, imageById!.Id);
    }

    [Fact]
    public async Task GetIDInImagesAsync_ReturnsNull_IfNotExist()
    {
        await using var db = CreateDbContext();

        var imageService = new ImageService(db);
        var nonExistingImageId = 9999;


        var result = await imageService.GetImageByIdAsync(nonExistingImageId);


        Assert.Null(result);
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_returnsTheListOfImages()
    {
        await using var db = CreateDbContext();
        var citizen = await CitizenSeeder.SeedCitizenAsync(db, seed: 1);
        var seededImages = await ImageSeeder.SeededManyForCitizenImagesAsync(db, seed: Seed, count: 3, citizen.Id);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(citizen.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seededImages.Select(x => x.Image.Id).OrderBy(id => id),
            result.Select(i => i.Id).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_UserNotFound_ReturnsNull()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(42);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetImagesByCitizenIdAsync_ReturnsOnlyImagesRelatedToCitizen()
    {
        await using var db = CreateDbContext();
        var citizen1 = await CitizenSeeder.SeedCitizenAsync(db, seed: Seed + 1);
        var citizen2 = await CitizenSeeder.SeedCitizenAsync(db, seed: Seed + 2);
        var seededImages = await ImageSeeder.SeededManyForCitizenImagesAsync(db, seed: 1, count: 3, citizen1.Id);
        var irrelevantImages = await ImageSeeder.SeededManyForCitizenImagesAsync(db, seed: 1, count: 3, citizen2.Id);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(citizen1.Id);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seededImages.Select(x => x.Image.Id).OrderBy(id => id),
            result.Select(i => i.Id).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task DeleteImageAsync_DeleteImage()
    {
        await using var db = CreateDbContext();

        var seededImage = await ImageSeeder.SeedAsync(db, Seed);
        var image = seededImage.Image;

        var service = new ImageService(db);

        Assert.True(await service.DeleteImageAsync(image.Id));

        var deletedImage = await db.Images
          .SingleOrDefaultAsync(c => c.Id == image.Id);

        Assert.Null(deletedImage);

    }

    [Fact]
    public async Task DeleteImageAsync_DeletesRightImage()
    {
        await using var db = CreateDbContext();

        var seededImages = await ImageSeeder.SeedManyAsync(db, Seed, 2);
        var image1 = seededImages[0].Image;
        var image2 = seededImages[1].Image;

        Assert.NotNull(image1);
        Assert.NotNull(image2);

        var service = new ImageService(db);

        Assert.True(await service.DeleteImageAsync(image1.Id));

        var deletedImage = await db.Images
            .SingleOrDefaultAsync(c => c.Id == image1.Id);
        Assert.Null(deletedImage);

        var remainingImage = await db.Images
            .SingleOrDefaultAsync(i => i.Id == image2.Id);

        Assert.NotNull(remainingImage);

    }

    [Fact]
    public async Task DeleteImageAsync_ReturnsFalseIfIdDontExists()
    {
        await using var db = CreateDbContext();
        var service = new ImageService(db);
        var nonExistingImageId = 9999;

        var result = await service.DeleteImageAsync(nonExistingImageId);

        Assert.False(result);
    }


    [Fact]
    public async Task PostImageAsync_PostImage()
    {
        await using var db = CreateDbContext();
        var request = ImageSeeder.CreateRequestBody(Seed);
        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            Assert.Equal(request.Name, created!.Name);
            Assert.Equal(request.OrganizationId, created.OrganizationId);
            Assert.Equal(request.CitizenId, created.CitizenId);
            Assert.EndsWith(".png", created.Path);
            Assert.True(File.Exists(created.Path));
            var stored = await db.Images.SingleOrDefaultAsync(i => i.Id == created.Id);
            Assert.NotNull(stored);
            Assert.Equal(created.Path, stored!.Path);
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }

    [Fact]
    public async Task PostImageAsync_PostsTheRightImage()
    {
        await using var db = CreateDbContext();
        var request = ImageSeeder.CreateRequestBody(Seed);
        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            var allImages = await db.Images.ToListAsync();
            var stored = Assert.Single(allImages);
            Assert.Equal(created!.Id, stored.Id);
            Assert.Equal(request.Name, stored.Name);
            Assert.Equal(request.OrganizationId, stored.OrganizationId);
            Assert.Equal(request.CitizenId, stored.CitizenId);

            Assert.Equal(created.Path, stored.Path);
            Assert.True(File.Exists(stored.Path));
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }

    [Fact]
    public async Task PostImageAsync_requestKnowingClassId()
    {
        await using var db = CreateDbContext();
        var request = ImageSeeder.CreateRequestBody(Seed);
        // setting citizenID and OrganizationId to null
        request.CitizenId = null;
        request.OrganizationId = null;
        var service = new ImageService(db);
        var created = await service.CreateImagesAsync(request);
        try
        {
            Assert.NotNull(created);
            var image = await db.Images.ToListAsync();
            var stored = Assert.Single(image);
            Assert.Equal(created!.Id, stored.Id);
            Assert.Equal(request.Name, stored.Name);
            Assert.Equal(request.OrganizationId, stored.OrganizationId);
            Assert.Equal(request.CitizenId, stored.CitizenId);

            Assert.Equal(created.Path, stored.Path);
            Assert.True(File.Exists(stored.Path));
        }
        finally
        {
            if (created?.Path is not null && File.Exists(created.Path))
            {
                File.Delete(created.Path);
            }
        }
    }
}