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
        // No patch - All fields of UserRoles models are without setters. (Use PostUserRole instead)
        group.MapDelete("/{id:int}/userRoles", DeleteUserRole); //200 & 404
        group.MapPost("/{id:int}/userRoles", PostUserRole); //200 & 404

    }

    private static async Task<Results<Ok<ResponseGetNonCitizenDTO>, NotFound>> GetNonCitizen(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.GetNonCitizenByIdAsync(nonCitizenId);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
    }

    private static async Task<Results<Ok<ResponseGetNonCitizenDTO>, NotFound>> PatchNonCitizen(int nonCitizenId, [FromBody] RequestPatchNonCitizenDTO requestPatchNonCitizenDTO, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PatchNonCitizenAsync(nonCitizenId, requestPatchNonCitizenDTO);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
    }

    private static async Task<Results<NoContent, NotFound>> DeleteNonCitizen(int nonCitizenId, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.DeleteNonCitizenAsync(nonCitizenId);
        return nonCitizen ? TypedResults.NoContent() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ResponseGetNonCitizenDTO>, NotFound>> PostNonCitizen(int nonCitizenId, [FromBody] RequestPostNonCitizenDTO requestPostNonCitizenDTO, NonCitizenService nonCitizenService)
    {
        var nonCitizen = await nonCitizenService.PostNonCitizenAsync(requestPostNonCitizenDTO);
        return nonCitizen == null ? TypedResults.NotFound() : TypedResults.Ok(nonCitizen);
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

    private static async Task<Results<Ok<ResponseNonCitizenUserRolesDTO>, NotFound>> PostUserRole(int nonCitizenId, [FromBody] RequestNonCitizenUserRoleDTO requestNonCitizenUserRoleDTO, NonCitizenService nonCitizenService)
    {
        var userRoles = await nonCitizenService.PostUserRoleAsync(nonCitizenId, requestNonCitizenUserRoleDTO);
        return userRoles == null ? TypedResults.NotFound() : TypedResults.Ok(userRoles);
    }



}