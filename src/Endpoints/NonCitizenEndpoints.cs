using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class NonCitizenEndpoints
{
    public static void MapNonCitizenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nonCitizens");

        group.MapGet("/{id:int}", GetNonCitizen); // 200 & 404
        group.MapPatch("/{id:int}", PatchNonCitizen); //200 & 404
        group.MapDelete("/{id:int}", DeleteNonCitizen); //200 & 404
        group.MapPost("/", PostNonCitizen); //200 & 404

        group.MapGet("/{id:int}/userRoles", GetUserRoles); // 200 & 404
        // No patch - All fields of UserRoles model are without setters. (Use PostUserRole instead)
        group.MapDelete("/{id:int}/userRoles", DeleteUserRole); //200 & 404
        group.MapPost("/{id:int}/userRoles", PostUserRole); //200 & 404

        group.MapGet("/{id:int}/guardianships", GetGuardianships); // 200 & 404
        // No patch - All fields of Guardianship model are without setters. (Use PostUserGuardianship instead)
        group.MapDelete("/{id:int}/guardianship", DeleteGuardianship); //200 & 404
        group.MapPost("/{id:int}/guardianship", PostGuardianship); //200 & 404

    }

    private static async Task<Results<Ok<ResponseNonCitizenDTO>, NotFound>> GetNonCitizen(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
    }

    private static async Task<Results<NotFound, UnauthorizedHttpResult, BadRequest, Conflict, Ok<ResponseNonCitizenDTO>>> PatchNonCitizen(int nonCitizenId, [FromBody] RequestPatchNonCitizenDTO requestPatchNonCitizenDTO, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PatchNonCitizenAsync(nonCitizenId, requestPatchNonCitizenDTO);
        return nonCitizen.Item1 switch
        {
            HttpReponseType.NotFound => TypedResults.NotFound(),
            HttpReponseType.Conflict => TypedResults.Conflict(),
            HttpReponseType.Unauthorized => TypedResults.Unauthorized(),
            HttpReponseType.Ok => TypedResults.Ok(nonCitizen.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<NoContent, NotFound>> DeleteNonCitizen(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.DeleteNonCitizenAsync(nonCitizenId);
        return nonCitizen ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Conflict, NotFound, Ok<ResponseNonCitizenDTO>, BadRequest>> PostNonCitizen([FromBody] RequestPostNonCitizenDTO requestPostNonCitizenDTO, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PostNonCitizenAsync(requestPostNonCitizenDTO);
        return nonCitizen.Item1 switch
        {
            HttpReponseType.Conflict => TypedResults.Conflict(),
            HttpReponseType.NotFound => TypedResults.NotFound(),
            HttpReponseType.Ok => TypedResults.Ok(nonCitizen.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<Ok<ResponseNonCitizenUserRolesDTO>, NotFound>> GetUserRoles(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var userRoles = await nonCitizenService.GetUserRolesAsync(nonCitizenId);
        return userRoles == null ? TypedResults.NotFound() : TypedResults.Ok(userRoles);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUserRole(int nonCitizenId, [FromBody] RequestNonCitizenUserRoleDTO requestNonCitizenUserRoleDTO, NonCitizenService nonCitizenService)
    {
        var userRole = await nonCitizenService.DeleteUserRoleAsync(nonCitizenId, requestNonCitizenUserRoleDTO);
        return userRole ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseNonCitizenUserRolesDTO>, NotFound, Conflict, BadRequest>> PostUserRole(int nonCitizenId, [FromBody] RequestNonCitizenUserRoleDTO requestNonCitizenUserRoleDTO, NonCitizenService nonCitizenService)
    {
        var userRole = await nonCitizenService.PostUserRoleAsync(nonCitizenId, requestNonCitizenUserRoleDTO);
        return userRole.Item1 switch
        {
            HttpReponseType.NotFound => TypedResults.NotFound(),
            HttpReponseType.Conflict => TypedResults.Conflict(),
            HttpReponseType.Ok => TypedResults.Ok(userRole.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<Ok<ResponseGuardianshipsDTO>, NotFound>> GetGuardianships(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var guardianships = await nonCitizenService.GetGuardianshipsAsync(nonCitizenId);
        return guardianships == null ? TypedResults.NotFound() : TypedResults.Ok(guardianships);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteGuardianship(int nonCitizenId, [FromBody] RequestGuardianshipDTO requestGuardianshipDTO, NonCitizenService nonCitizenService)
    {
        var guardianship = await nonCitizenService.DeleteGuardianshipAsync(nonCitizenId, requestGuardianshipDTO);
        return guardianship ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseGuardianshipsDTO>, NotFound, Conflict, BadRequest>> PostGuardianship(int nonCitizenId, [FromBody] RequestGuardianshipDTO requestGuardianshipDTO, NonCitizenService nonCitizenService)
    {
        var guardianships = await nonCitizenService.PostGuardianshipAsync(nonCitizenId, requestGuardianshipDTO);
        return guardianships.Item1 switch
        {
            HttpReponseType.NotFound => TypedResults.NotFound(),
            HttpReponseType.Conflict => TypedResults.Conflict(),
            HttpReponseType.Ok => TypedResults.Ok(guardianships.Item2),
            _ => TypedResults.BadRequest()
        };
    }


}