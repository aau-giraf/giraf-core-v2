using Microsoft.AspNetCore.Http.HttpResults;

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

        var SeededImage = await ImageSeeder.SeedAsync(db, Seed);

        var image = SeededImage.Image;

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


    // SKAL LAVES OM NÅR CITIZEN SEEDEREN BLIVER LAVET
    [Fact]
    public async Task GetImagesByCitizenIdAsync_returnsTheListOfImages()
    {
        await using var db = CreateDbContext();
        //await SeedCitizenAsync(db, 1);
        var seeded = await ImageSeeder.SeededManyForCitizenImagesAsync(db, seed: 1, count: 3, citizenId: 1);
        var service = new ImageService(db);

        var result = await service.GetImagesByCitizenIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(3, result!.Count);
        Assert.Equal(
            seeded.Select(x => x.Image.Id).OrderBy(id => id),
            result.Select(i => i.Id).OrderBy(id => id)
        );
    }

    [Fact]
    public async Task DeleteImageAsync_DeleteImage()
    {
        await using var db = CreateDbContext();

        var SeededImage = await ImageSeeder.SeedAsync(db, Seed);
        var image = SeededImage.Image;

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

        var SeededImages = await ImageSeeder.SeedManyAsync(db, Seed, 2);
        var image1 = SeededImages[0].Image;
        var image2 = SeededImages[1].Image;

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
        Directory.CreateDirectory("images");
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
        Directory.CreateDirectory("images");
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
}