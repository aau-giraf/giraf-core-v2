using Microsoft.AspNetCore.StaticFiles;

public static class ColorEndpoints
{
    public static void MapColorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/colors");

        group.MapGet("/", GetColors);
        
    }

    private static async Task<IResult> GetColors(ColorService colorService)
    {
        var colors = await colorService.GetColorsAsync();
        return Results.Ok(colors);
    }


}