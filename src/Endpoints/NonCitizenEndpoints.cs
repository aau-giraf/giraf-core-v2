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
        // No patch - All fields of EmployeeRoles model are without setters. (Use PostUserRole instead)
        group.MapDelete("/{nonCitizenId}/userRoles", DeleteUserRoleAsync);
        group.MapPost("/{nonCitizenId}/userRoles", PostUserRoleAsync);

        group.MapGet("/{nonCitizenId}/guardianships", GetGuardianshipsAsync);
        // No patch - All fields of Guardianship model are without setters. (Use PostUserGuardianship instead)
        group.MapDelete("/{nonCitizenId}/guardianships", DeleteGuardianshipAsync);
        group.MapPost("/{nonCitizenId}/guardianships", PostGuardianshipAsync);

    }

    private static async Task<Results<Ok<GetNonCitizenResponseDTO>, NotFound>> GetNonCitizenAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
    }

    private static async Task<Results<NotFound, UnauthorizedHttpResult, BadRequest, Conflict, Ok<GetNonCitizenResponseDTO>>> PatchNonCitizenAsync(int nonCitizenId, [FromBody] PatchNonCitizenRequestDTO patchNonCitizen, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PatchNonCitizenAsync(nonCitizenId, patchNonCitizen);
        return nonCitizen.Item1 switch
        {
            HttpResponseType.NotFound => TypedResults.NotFound(),
            HttpResponseType.Conflict => TypedResults.Conflict(),
            HttpResponseType.Unauthorized => TypedResults.Unauthorized(),
            HttpResponseType.Ok => TypedResults.Ok(nonCitizen.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<NoContent, NotFound>> DeleteNonCitizenAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.DeleteNonCitizenAsync(nonCitizenId);
        return nonCitizen ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Conflict, NotFound, Ok<GetNonCitizenResponseDTO>, BadRequest>> PostNonCitizenAsync([FromBody] PostNonCitizenRequestDTO postNonCitizen, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PostNonCitizenAsync(postNonCitizen);
        return nonCitizen.Item1 switch
        {
            HttpResponseType.Conflict => TypedResults.Conflict(),
            HttpResponseType.NotFound => TypedResults.NotFound(),
            HttpResponseType.Ok => TypedResults.Ok(nonCitizen.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<Ok<GetNonCitizenUserRolesResponseDTO>, NotFound>> GetUserRolesAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var userRoles = await nonCitizenService.GetUserRolesAsync(nonCitizenId);
        return userRoles == null ? TypedResults.NotFound() : TypedResults.Ok(userRoles);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteUserRoleAsync(int nonCitizenId, [FromBody] NonCitizenUserRoleRequestDTO nonCitizenUserRole, NonCitizenService nonCitizenService)
    {
        var userRole = await nonCitizenService.DeleteUserRoleAsync(nonCitizenId, nonCitizenUserRole);
        return userRole ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<GetNonCitizenUserRolesResponseDTO>, NotFound, Conflict, BadRequest>> PostUserRoleAsync(int nonCitizenId, [FromBody] NonCitizenUserRoleRequestDTO nonCitizenUserRole, NonCitizenService nonCitizenService)
    {
        var userRole = await nonCitizenService.PostUserRoleAsync(nonCitizenId, nonCitizenUserRole);
        return userRole.Item1 switch
        {
            HttpResponseType.NotFound => TypedResults.NotFound(),
            HttpResponseType.Conflict => TypedResults.Conflict(),
            HttpResponseType.Ok => TypedResults.Ok(userRole.Item2),
            _ => TypedResults.BadRequest()
        };
    }

    private static async Task<Results<Ok<GetGuardianshipsResponseDTO>, NotFound>> GetGuardianshipsAsync(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var guardianships = await nonCitizenService.GetGuardianshipsAsync(nonCitizenId);
        return guardianships == null ? TypedResults.NotFound() : TypedResults.Ok(guardianships);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteGuardianshipAsync(int nonCitizenId, [FromBody] GuardianshipRequestDTO guardianshipRequest, NonCitizenService nonCitizenService)
    {
        var guardianship = await nonCitizenService.DeleteGuardianshipAsync(nonCitizenId, guardianshipRequest);
        return guardianship ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<GetGuardianshipsResponseDTO>, NotFound, Conflict, BadRequest>> PostGuardianshipAsync(int nonCitizenId, [FromBody] GuardianshipRequestDTO guardianshipRequest, NonCitizenService nonCitizenService)
    {
        var guardianships = await nonCitizenService.PostGuardianshipAsync(nonCitizenId, guardianshipRequest);
        return guardianships.Item1 switch
        {
            HttpResponseType.NotFound => TypedResults.NotFound(),
            HttpResponseType.Conflict => TypedResults.Conflict(),
            HttpResponseType.Ok => TypedResults.Ok(guardianships.Item2),
            _ => TypedResults.BadRequest()
        };
    }


}