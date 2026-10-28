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
        groupOrg.MapGet("/{orgId}/citizens", GetCitizens);
    }

    private static async Task<Results<Ok<CitizenDTO>, NotFound>> GetCitizen(int citizenId, CitizenService citizenService)
    {
        var citizen = await citizenService.GetCitizenAsync(citizenId);
        if (citizen is null)
        {
            return TypedResults.NotFound();
        }
        
        // Create a citizenDTO exposing only UserId, GuardianId and ClassId
        var dto = new CitizenDTO(citizen.UserId, citizen.GuardianId,  citizen.ClassId);
        return TypedResults.Ok(dto);
    }

    private static async Task<Results<Ok<List<CitizenDTO>>, NotFound>> GetCitizens(int orgId, CitizenService citizenService)
    {
        var citizens = await citizenService.GetCitizensAsync(orgId);
        if (citizens is null)
        {
            return TypedResults.NotFound();
        }
        
        // Goes over the citizens in the org and lists them with their userid, guardianid and classid
        var dtos = citizens
            .Select(c => new CitizenDTO(c.UserId, c.GuardianId, c.ClassId))
            .ToList();
        return TypedResults.Ok(dtos);
    }

    private static async Task<Results<Created<CitizenDTO>, BadRequest>> CreateCitizen(CitizenService citizenService, [FromBody] CreateCitizenDTO createCitizen)
    {
        var citizen = await citizenService.CreateCitizenAsync(createCitizen);
        
        if (citizen is null)
        {
            return TypedResults.BadRequest();
        }
        
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