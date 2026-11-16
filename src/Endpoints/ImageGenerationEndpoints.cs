using System.Security.Claims;

using giraf_core_v2.DTOs.Images;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class ImageGenerateEndpoints
{
    public static void MapImageGenerateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/images");

        group.MapPost("/prompt", PostPromptForImage).RequireAuthorization();
        group.MapPost("/inappropriate", InappropriateImage);
        group.MapGet("/done/{ticketId}", IsImageDone).RequireAuthorization();
        group.MapPost("/generated", PostGeneratedImage).DisableAntiforgery();
    }

    private static int GetCurrentUserId(ClaimsPrincipal currentUser)
    {
        return int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in JWT claim!"));
    }

    private static async Task<Results<Ok<string>, ProblemHttpResult>> PostPromptForImage([FromBody] PostImagePromptDTO postImagePromptDto, ClaimsPrincipal currentUser, ImageService imageService)
    {
        var userId = GetCurrentUserId(currentUser);
        var ticketId = await imageService.GenerateImage(postImagePromptDto, userId);

        return ticketId is null ? TypedResults.Problem("Image generation is unavailable right now", statusCode: 503) : TypedResults.Ok(ticketId);
    }

    private static Ok InappropriateImage([FromBody] InappropriateImageDTO inappropriateImageDto, ImageService imageService)
    {
        imageService.MarkInappropriate(inappropriateImageDto.TicketId);
        return TypedResults.Ok();
    }

    private static async Task<Ok<ReqIsImageDoneDTO>> IsImageDone(string ticketId, ImageService imageService)
    {
        var req = await imageService.IsImageDone(ticketId);

        return TypedResults.Ok(req);
    }

    private static async Task<Results<Ok<int>, BadRequest<string>>> PostGeneratedImage([FromForm] PostImageDTO imageRequest, ImageService imageService)
    {
        // An AI-generated image must belong to a ticket, otherwise the client can never find it
        if (string.IsNullOrWhiteSpace(imageRequest.TicketId))
        {
            return TypedResults.BadRequest("TicketId is required");
        }

        var image = await imageService.CreateImagesAsync(imageRequest);

        return image is null
            ? TypedResults.BadRequest("Could not find an organization for the image")
            : TypedResults.Ok(image.Id);
    }
}