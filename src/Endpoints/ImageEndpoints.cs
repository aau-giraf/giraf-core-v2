using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc; 
namespace giraf_core_v2.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication application)
        {
            var group = application.MapGroup("/images");

            group.MapGet("/{image_id}", GetIDInImages);
            group.MapPost("/{image_id}/sound");
            group.MapDelete("/{image_id}/");

        } 
        
    private static async Task<Results<Ok<Image>, NotFound>> GetIDInImages(int image_id, ImageService ImageService)
    {
        var classes = await ImageService.GetIDInImagesAsync(image_id);
        return classes == null ? TypedResults.NotFound() : TypedResults.Ok(classes);
    }

    private static async Task<IResult> PostImage(int image_id, )
    {
        
    } 
}