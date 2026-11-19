using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace giraf_core_v2.Endpoints;

public static class CitizenEndpoints
{
    public static void MapCitizenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/citizens");

        group.MapGet("/{citizenId}", GetCitizenAsync);
        group.MapPost("", CreateCitizenAsync);
        group.MapPatch("/{citizenId}", UpdateCitizenAsync);
        group.MapDelete("/{citizenId}", DeleteCitizenAsync);
    }

    private static async Task<Results<Ok<GetCitizenResponseDTO>, NotFound>> GetCitizenAsync(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);

        return citizen != null ? TypedResults.Ok(citizen) : TypedResults.NotFound();
    }

    private static async Task<Results<Created<GetCitizenResponseDTO>, BadRequest>> CreateCitizenAsync(CitizenService citizenService, [FromBody] CreateCitizenRequestDTO createCitizen)
    {
        var citizen = await citizenService.CreateCitizenAsync(createCitizen);

        return citizen != null ? TypedResults.Created("/citizens", citizen) : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<GetCitizenResponseDTO>, NotFound, BadRequest>> UpdateCitizenAsync(int citizenId, CitizenService citizenService, [FromBody] UpdateCitizenRequestDTO? updateCitizen)
    {
        if (updateCitizen is null)
        {
            return TypedResults.BadRequest();
        }

        var citizen = await citizenService.PatchCitizenAsync(citizenId, updateCitizen);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCitizenAsync(int citizenId, CitizenService citizenService)
    {
        var deleted = await citizenService.DeleteCitizenAsync(citizenId);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}