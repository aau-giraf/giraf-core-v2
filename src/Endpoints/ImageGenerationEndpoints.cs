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
        group.MapPost("/inappropiate", InappropiateImage);
        group.MapGet("/done/{ticketId}", IsImageDone);
    }

    private static int GetCurrentUserId(ClaimsPrincipal currentUser)
    {
        return int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in JWT claim!"));
    }

    private static async Task<Results<Ok<string>, UnauthorizedHttpResult>> PostPromptForImage([FromBody] PostImagePromptDTO postImagePromptDto, ClaimsPrincipal currentUser, ImageService imageService)
    {
        // Auth check that returns 401 if the user if not eligible to create an image
        var userId = GetCurrentUserId(currentUser);

        var genImage = await imageService.GenerateImage(postImagePromptDto, userId);

        var ticketId = genImage.TicketId;

        return TypedResults.Ok(ticketId);
    }

    private static async Task<Results<Ok, BadRequest<string>>> InappropiateImage([FromBody] InappropiateImageDTO inappropiateImageDto, ImageService imageService)
    {
        var req = await imageService.InappropiateImage(inappropiateImageDto);
        if (req == null)
        {
            return TypedResults.BadRequest("TicketId cannot be null and Inappropriate cannot be false");
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok<ReqIsImageDoneDTO>, NotFound>> IsImageDone(string ticketId, ImageService imageService)
    {
        var req = await imageService.IsImageDone(ticketId);

        if (req == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(req);
    }
}