using System.Security.Claims;

using giraf_core_v2.DTOs.Images;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class ImageGenerateEndpoints
{
    public static void MapImageGenerateEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/generate");

        // group.MapGet("/{ticketId}", GetImageByTicketId);
        group.MapPost("/image", PostPromptForImage);

    }
    
    private static int GetCurrentUserId(ClaimsPrincipal currentUser)
    {
        return int.Parse(currentUser.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? throw new Exception("User ID not found in JWT claim!"));
    }

    private static async Task<Results<Ok<string>, UnauthorizedHttpResult>> PostPromptForImage([FromBody] PostImagePromptDTO postImagePromptDto, ImageService imageService)
    {
        // Auth check that returns Unauthorized if the user if not eligible to create an image.
        //var user = await GetCurrentUserId();
        
        var genImage = await imageService.GenerateImage(postImagePromptDto);

        var ticketId = genImage.TicketId;

        return TypedResults.Ok(ticketId);
    }
}