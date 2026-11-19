using giraf_core_v2.DTOs.Images;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;


public static class ImageEndpoints
{
    public static void MapImageEndpoints(this WebApplication application)
    {
        var group = application.MapGroup("/images");

        group.MapGet("/{image_id}", GetImageById);
        group.MapGet("/", GetAllImages);
        group.MapGet("/Citizen/{cit_id}", GetImagesByCitizenId);
        group.MapPost("/", PostImage).DisableAntiforgery(); // needed for us to sent a Form via Postman/bruno
        group.MapDelete("/{image_id}", DeleteImage);
        group.MapPost("/generated", PostGeneratedImage).DisableAntiforgery();
        group.MapGet("/done/{ticketId}", IsImageDone).RequireAuthorization();
    }
    // GET imagees by ID
    private static async Task<Results<Ok<Image>, NotFound>> GetImageById(int image_id, ImageService imageService)
    {
        var image = await imageService.GetImageByIdAsync(image_id);
        return image == null ? TypedResults.NotFound() : TypedResults.Ok(image);
    }

    // GET images with a specific Citizen id and returns a list with all images
    private static async Task<Results<Ok<List<Image>>, NotFound>> GetImagesByCitizenId(int cit_id, ImageService imageService)
    {
        var images = await imageService.GetImagesByCitizenIdAsync(cit_id);
        return images is null ? TypedResults.NotFound() : TypedResults.Ok(images);
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

    // POST image
    // Needs to be from form for us to upload pictures
    private static async Task<Results<Created<Image>, BadRequest>> PostImage([FromForm] PostImageDTO? imageRequest, ImageService imageService)
    {
        if (imageRequest is null)
        {
            return TypedResults.BadRequest();
        }

        var image = await imageService.CreateImagesAsync(imageRequest);
        return image == null ? TypedResults.BadRequest() : TypedResults.Created($"/images/{image.Id}", image);
    }
    
    // This endpoint is similar to the "PostImage" yet there are some differences like this has to get a TicketId in the body
    private static async Task<Results<Created<Image>, BadRequest<string>>> PostGeneratedImage([FromForm] PostImageDTO imageRequest, ImageService imageService)
    {
        // An AI-generated image must belong to a ticket, otherwise the client can never find it
        if (string.IsNullOrWhiteSpace(imageRequest.TicketId))
        {
            return TypedResults.BadRequest("TicketId is required");
        }

        // Call the service with the imageRequest (DTO)
        var image = await imageService.CreateImagesAsync(imageRequest);

        return image is null
            ? TypedResults.BadRequest("Could not find an organization for the image")
            : TypedResults.Created($"/images/{image.Id}", image);
    }
    
    private static async Task<Ok<IsImageDoneRequestDTO>> IsImageDone(string ticketId, ImageService imageService)
    {
        var req = await imageService.IsImageDone(ticketId);

        return TypedResults.Ok(req);
    }
}
public record CreateImageRequest(string Name, int OrganizationId, int? CitizenId, string StorageKey, string FileName, string FileType, long SizeBytes);