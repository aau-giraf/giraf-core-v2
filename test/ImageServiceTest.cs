using Xunit.Abstractions;

public class ImageServiceTest : Testbase
{

    [Fact]
    public async Task GetIDInImagesAsync_ReturnsImage_WhenImageExists()
    {
    
        await using var db = CreateDbContext();
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
}