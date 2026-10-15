using Xunit.Abstractions;

public class ImageServiceTest : TestBase
{
    public ImageServiceTest(ITestOutputHelper output) : base(output) { }

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
}
