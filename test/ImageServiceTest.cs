using System.Data.Common;
using Xunit.Abstractions;

public class ImageServiceTest : TestBase
{
    public ImageServiceTest(ITestOutputHelper output) : base(output) {}



    [Fact]
    public async Task GetIDInImagesAsync_ReturnsImage_WhenImageExists()
    {
    
        await using var db = CreateDbContext();

        var SeededImage = await ImageSeeder.SeedAsync(db, Seed);

        var imageService = new ImageService(_testbase.DbContext);
        var existingImageId = 1; 

       
        var result = await imageService.GetIDInImagesAsync(existingImageId);

       
        Assert.NotNull(result);
        Assert.Equal(existingImageId, result.Id);
    }

    [Fact]
    public async Task GetIDInImagesAsync_ReturnsNull_IfNotExist()
    {
       
        var imageService = new ImageService(_testbase.DbContext);
        var nonExistingImageId = 9999;

        
        var result = await imageService.GetIDInImagesAsync(nonExistingImageId);

        
        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteImageAsync_DeleteImage()
    {
      await using var db = CreateDbContext();
      
      var SeededImage = await ImageSeeder.SeedAsync(db, Seed);
      var image = SeededImage; 

      var service = new ImageService(db);

      Assert.True(await service.DeleteImageAsync(image.id));

      var deletedImage = await db.Images
        .SingleOrDefaultAsync(c => c.id == image.id);

      Assert.Null(deletedImage);

    }

    [Fact]
    public async Task DeleteImageAsync_DeletesRightImage()
    {
        await using db = CreateDbContext();

        var SeededImage = await ImageSeeder.SeedAsync(db, Seed);
        var image1 = SeededImage[0].Image;
        var image2 = SeededImage[1].Image;

        Assert.NotNull(image1);
        Assert.NotNull(image2);
    }
}