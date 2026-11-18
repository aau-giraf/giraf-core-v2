using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public static class NonCitizenEndpoints
{
    public static void MapNonCitizenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nonCitizens");

        group.MapGet("/{nonCitizenId}", GetNonCitizenAsync);
        group.MapPatch("/{nonCitizenId}", PatchNonCitizenAsync);
        group.MapDelete("/{nonCitizenId}", DeleteNonCitizenAsync);
        group.MapPost("/", PostNonCitizenAsync);

        group.MapGet("/{nonCitizenId}/userRoles", GetUserRolesAsync);
        // No patch - All fields of UserRoles model are without setters. (Use PostUserRole instead)
        group.MapDelete("/{nonCitizenId}/userRoles", DeleteUserRoleAsync);
        group.MapPost("/{nonCitizenId}/userRoles", PostUserRoleAsync);

        group.MapGet("/{nonCitizenId}/guardianships", GetGuardianshipsAsync);
        // No patch - All fields of Guardianship model are without setters. (Use PostUserGuardianship instead)
        group.MapDelete("/{nonCitizenId}/guardianships", DeleteGuardianshipAsync);
        group.MapPost("/{nonCitizenId}/guardianships", PostGuardianshipAsync);

    }

    private static async Task<Results<Ok<NonCitizenResponseDTO>, NotFound>> GetNonCitizenAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
    }

    private static async Task<Results<NotFound, UnauthorizedHttpResult, BadRequest, Conflict, Ok<NonCitizenResponseDTO>>> PatchNonCitizenAsync(int nonCitizenId, [FromBody] PatchNonCitizenRequestDTO requestPatchNonCitizenDTO, NonCitizenService nonCitizenService)
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

    private static async Task<Results<NoContent, NotFound>> DeleteNonCitizenAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.DeleteNonCitizenAsync(nonCitizenId);
        return nonCitizen ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Conflict, NotFound, Ok<NonCitizenResponseDTO>, BadRequest>> PostNonCitizenAsync([FromBody] PostNonCitizenRequestDTO requestPostNonCitizenDTO, NonCitizenService nonCitizenService)
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

    private static async Task<Results<Ok<NonCitizenUserRolesResponseDTO>, NotFound>> GetUserRolesAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var userRoles = await nonCitizenService.GetUserRolesAsync(nonCitizenId);
        return userRoles == null ? TypedResults.NotFound() : TypedResults.Ok(userRoles);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUserRoleAsync(int nonCitizenId, [FromBody] NonCitizenUserRoleRequestDTO requestNonCitizenUserRoleDTO, NonCitizenService nonCitizenService)
    {
        var userRole = await nonCitizenService.DeleteUserRoleAsync(nonCitizenId, requestNonCitizenUserRoleDTO);
        return userRole ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<NonCitizenUserRolesResponseDTO>, NotFound, Conflict, BadRequest>> PostUserRoleAsync(int nonCitizenId, [FromBody] NonCitizenUserRoleRequestDTO requestNonCitizenUserRoleDTO, NonCitizenService nonCitizenService)
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

    private static async Task<Results<Ok<GuardianshipsResponseDTO>, NotFound>> GetGuardianshipsAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var guardianships = await nonCitizenService.GetGuardianshipsAsync(nonCitizenId);
        return guardianships == null ? TypedResults.NotFound() : TypedResults.Ok(guardianships);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteGuardianshipAsync(int nonCitizenId, [FromBody] GuardianshipRequestDTO requestGuardianshipDTO, NonCitizenService nonCitizenService)
    {
        var guardianship = await nonCitizenService.DeleteGuardianshipAsync(nonCitizenId, requestGuardianshipDTO);
        return guardianship ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<GuardianshipsResponseDTO>, NotFound, Conflict, BadRequest>> PostGuardianshipAsync(int nonCitizenId, [FromBody] GuardianshipRequestDTO requestGuardianshipDTO, NonCitizenService nonCitizenService)
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