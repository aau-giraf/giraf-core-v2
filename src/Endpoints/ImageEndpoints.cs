using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;


public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/images");

        group.MapGet("/{imageId}", GetImageByIdAsync);
        group.MapGet("/", GetAllImagesAsync);
        group.MapGet("/citizens/{citizenId}", GetImagesByCitizenIdAsync);
        group.MapPost("/", PostImageAsync).DisableAntiforgery(); // needed for us to sent a Form via Postman/bruno
        group.MapDelete("/{imageId}", DeleteImageAsync);
        group.MapGet("/{filename}/hejsa.png", showImage);
    }
    // GET imagees by ID
    private static async Task<Results<Ok<Image>, NotFound>> GetImageByIdAsync(int imageId, ImageService imageService)
    {
        var image = await imageService.GetImageByIdAsync(imageId);
        return image == null ? TypedResults.NotFound() : TypedResults.Ok(image);
    }

    // GET images with a specific Citizen id and returns a list with all images
    private static async Task<Results<Ok<List<Image>>, NotFound>> GetImagesByCitizenIdAsync(int citizenId, ImageService imageService)
    {
        var images = await imageService.GetImagesByCitizenIdAsync(citizenId);
        return images is null ? TypedResults.NotFound() : TypedResults.Ok(images);
    }

    // GET all images
    private static async Task<Ok<List<Image>>> GetAllImagesAsync(ImageService imageService)
    {
        var images = await imageService.GetAllImagesAsync();
        return TypedResults.Ok(images);
    }

    // DELETE image by ID
    private static async Task<Results<NoContent, NotFound>> DeleteImageAsync(int imageId, ImageService imageService)
    {
        var result = await imageService.DeleteImageAsync(imageId);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    // POST image
    // Needs to be from form for us to upload pictures
    private static async Task<Results<Created<Image>, BadRequest>> PostImageAsync([FromForm] PostImageRequestDTO? imageRequest, ImageService imageService)
    {
        if (imageRequest is null)
        {
            return TypedResults.BadRequest();
        }

        var image = await imageService.CreateImagesAsync(imageRequest);
        return image == null ? TypedResults.BadRequest() : TypedResults.Created($"/images/{image.Id}", image);
    }

    private static IResult showImage(string filename)
    {
        return Results.File(Path.GetFullPath(Path.Combine("images", filename)));
    }

}