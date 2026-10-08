using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; 
namespace giraf_core_v2.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication application)
        {
            var group = application.MapGroup("/images");

            group.MapGet("/{image_id}", GetIDInImages);
            group.MapGet("/", GetAllImages);
            //group.MapPost("/", PostImage);
            //group.MapPost("/{image_id}/sound");
            group.MapDelete("/{image_id}", DeleteImage);

        } 
        
    // GET imagees by ID
    private static async Task<Results<Ok<Image>, NotFound>> GetIDInImages(int image_id, ImageService imageService)
    {
        var image = await imageService.GetIDInImagesAsync(image_id);
        return image == null ? TypedResults.NotFound() : TypedResults.Ok(image);
    }

    // GET all images
    private static async Task<Ok<List<Image>>> GetAllImages(ImageService imageService)
    {
        var images = await imageService.GetAllImagesAsync();
        return TypedResults.Ok(images);
    }

    // DELETE image by ID
    private static async Task<Results<NoContent, NotFound>> DeleteImage(int image_id, ImageService imageService)
    {
        var result = await imageService.DeleteImageAsync(image_id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    //private static async Task<Created<Image>> PostImage(ImageService ImageService)
    //{
        //var image = await ImageService.CreateImagesAsync();
        //return TypedResults.Created($"/images/{image.Id}", image);  
    //} 
}