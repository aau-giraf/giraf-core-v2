using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;


public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/images");

        group.MapGet("/Citizen/{cit_id}", GetImagesByCitizenId);
        group.MapGet("/Organization/{organizationId}", GetImagesByOrganizationId);
        group.MapGet("/Class/{classId}", GetImagesByClassId);
        group.MapPost("/", PostImage).DisableAntiforgery(); // needed for us to sent a Form via Postman/bruno
        group.MapDelete("/{image_id}", DeleteImage);
    }

    // GET images with a specific Citizen id and returns a list with all images
    private static async Task<Results<Ok<List<ImageResponseDTO>>, NotFound>> GetImagesByCitizenId(int cit_id, ImageService imageService)
    {
        var images = await imageService.GetImagesByCitizenIdAsync(cit_id);
        return images is null ? TypedResults.NotFound() : TypedResults.Ok(images);
    }

    private static async Task<Results<Ok<List<ImageResponseDTO>>, NotFound>> GetImagesByClassId(int classId, ImageService imageService)
    {
        var images = await imageService.GetImagesByClassIdAsync(classId);
        return images is null ? TypedResults.NotFound() : TypedResults.Ok(images);
    }

    private static async Task<Results<Ok<List<ImageResponseDTO>>, NotFound>> GetImagesByOrganizationId(int organizationId, ImageService imageService)
    {
        var images = await imageService.GetImagesByOrganizationIdAsync(organizationId);
        return images is null ? TypedResults.NotFound() : TypedResults.Ok(images);
    }

    // DELETE image by ID
    private static async Task<Results<NoContent, NotFound>> DeleteImage(int image_id, ImageService imageService)
    {
        var result = await imageService.DeleteImageAsync(image_id);
        return result ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    // POST image
    // Needs to be from form for us to upload pictures
    private static async Task<Results<Created<CreateImageResponseDTO>, BadRequest>> PostImage([FromForm] PostImageDTO? imageRequest, ImageService imageService)
    {
        if (imageRequest is null)
        {
            return TypedResults.BadRequest();
        }

        var image = await imageService.CreateImagesAsync(imageRequest);
        return image == null ? TypedResults.BadRequest() : TypedResults.Created($"/images/{image.Title}", image);
    }

}