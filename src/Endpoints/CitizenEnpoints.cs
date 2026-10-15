using System.Net;
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
        group.MapPatch("/{citizenId}", UpdateCitizen);
        group.MapDelete("/{citizenId}", DeleteCitizen);
        groupOrg.MapPost("/citizens", CreateCitizen);
    }

    private static async Task<Results<Ok<Citizen>, NotFound>> GetCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<Created<CitizenDTO>, BadRequest>> CreateCitizen(CitizenService citizenService, [FromBody] CreateCitizenDTO createCitizen)
    {
		if (createCitizen is null)
        {
            return TypedResults.BadRequest();
        }

		var citizen = await citizenService.CreateCitizenAsync(createCitizen);

		var dto = new CitizenDTO(citizen.UserId, citizen.GuardianId,  citizen.ClassId);

		return TypedResults.Created($"/citizens/{dto.UserId}", dto);
    }

    private static async Task<Results<Ok<Citizen>, NotFound, BadRequest>> UpdateCitizen(int citizenId, CitizenService citizenService, [FromBody] UpdateCitizenDTO? updateCitizen)
    {
		if (updateCitizen is null)
        {
            return TypedResults.BadRequest();
        }

        var citizen = await citizenService.UpdateCitizenAsync(citizenId, updateCitizen);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCitizen(int citizenId, CitizenService citizenService)
    {
        var deleted = await citizenService.DeleteCitizenAsync(citizenId);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
