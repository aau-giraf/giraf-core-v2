using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
namespace giraf_core_v2.Endpoints;

public static class CitizenEndpoints
{
    public static void MapCitizenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/citizens");
        var groupOrg = app.MapGroup("/organizations");

        group.MapGet("/{citizenId}", GetCitizen);
        group.MapPost("", CreateCitizen);
        group.MapPatch("/{citizenId}", UpdateCitizen);
        group.MapDelete("/{citizenId}", DeleteCitizen);
    }

    private static async Task<Results<Ok<GetCitizenDTO>, NotFound>> GetCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);

        return citizen != null ? TypedResults.Ok(citizen) : TypedResults.NotFound();
    }

    private static async Task<Results<Created<GetCitizenDTO>, BadRequest>> CreateCitizen(CitizenService citizenService, [FromBody] CreateCitizenDTO createCitizen)
    {
        var citizen = await citizenService.CreateCitizenAsync(createCitizen);

        return citizen != null ? TypedResults.Created("/citizen", citizen) : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<GetCitizenDTO>, NotFound, BadRequest>> UpdateCitizen(int citizenId, CitizenService citizenService, [FromBody] UpdateCitizenDTO? updateCitizen)
    {
        if (updateCitizen is null)
        {
            return TypedResults.BadRequest();
        }

        var citizen = await citizenService.PatchCitizenAsync(citizenId, updateCitizen);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCitizen(int citizenId, CitizenService citizenService)
    {
        var deleted = await citizenService.DeleteCitizenAsync(citizenId);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}