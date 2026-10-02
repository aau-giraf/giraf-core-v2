namespace giraf_core_v2.Endpoints;
using System.Net;
using Microsoft.AspNetCore.Mvc;

public static class CitizenEnpoints
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

    private static async Task<IResult> GetCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);
        return citizen is null ? Results.NotFound() : Results.Ok(citizen);
    }

    private static async Task<IResult> CreateCitizen(int orgId, CitizenService citizenService)
    {
        var citizen = await citizenService.CreateCitizenAsync(orgId);
        return Results.Created($"/citizens/{citizen.CitizenId}", citizen);
    }

    private static async Task<IResult> UpdateCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.UpdateCitizenAsync(citizenId);
        return citizen is null ? Results.NotFound() : Results.Ok(citizen);
    }

    private static async Task<IResult> DeleteCitizen(int citizenId, CitizenService citizenService)
    {
        var deleted = await citizenService.DeleteCitizenAsync(citizenId);
        return deleted ? Results.NoContent() : Results.NotFound();
    }
}
