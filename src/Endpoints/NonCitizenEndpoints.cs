using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace giraf_core_v2.Endpoints;

public class NonCitizenEndpoints
{
    public static void MapNonCitizenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/nonCitizens");

        group.MapGet("/{id:int}", GetNonCitizen); // 200 & 404
        group.MapPatch("/{id}", UpdateNonCitizen); //200 & 404
        group.MapDelete("/{id}", DeleteNonCitizen); //200 & 404
        group.MapPost("/noncitizens", CreateNonCitizen); //201 & 400 & 409
    }

    private static async Task<Results<Created<CreateNonCitizenDTO>, BadRequest, Conflict>> GetNonCitizen(NonCitizenService nonCitizenService, RegisterUserDTO input)
    {
        var noncitizen = await nonCitizenService.CreateNonCitizenAsync(input);
        
        if (noncitizen is null)
        {
            return TypedResults.BadRequest();
        }
        
    }
}