using System.Net;
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
        groupOrg.MapPost("/{orgId}/citizens", CreateCitizen);
    }

    private static async Task<Results<Ok<Citizen>, NotFound>> GetCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);
        return citizen is null ? TypedResults.NotFound() : TypedResults.Ok(citizen);
    }

    private static async Task<Created<Citizen>> CreateCitizen(int orgId, CitizenService citizenService)
    {
        var citizen = await citizenService.CreateCitizenAsync(orgId);
        return TypedResults.Created($"/citizens/{citizen.CitizenId}", citizen);
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
