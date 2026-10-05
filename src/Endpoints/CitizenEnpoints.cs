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

    private static async Task<Results<Ok<Citizen>, NotFound>> GetCitizen(int userId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(userId);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<Created<Citizen>, BadRequest>> CreateCitizen(CitizenService citizenService, [FromBody] CreateCitizenDTO createCitizen)
    {
		if (createCitizen is null)
        {
            return TypedResults.BadRequest();
        }

		var citizen = await citizenService.CreateCitizenAsync(createCitizen);
        
		if (citizen is null)
        {
            return TypedResults.BadRequest();
        }

		return TypedResults.Created($"/citizens/{citizen.UserId}", citizen);
    }

    private static async Task<Results<Ok<Citizen>, NotFound>> UpdateCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.UpdateCitizenAsync(citizenId);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteCitizen(int citizenId, CitizenService citizenService)
    {
        var deleted = await citizenService.DeleteCitizenAsync(citizenId);
        return deleted ? TypedResults.NoContent() : TypedResults.NotFound();
    }
}
