using giraf_core_v2.Legacy_2.DTOs.LegacyUserDTO;
using giraf_core_v2.Migrations;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace giraf_core_v2.Legacy_2;


public static class LegacyChildrenEndpoints
{
    public static void MapLegacyChildrenEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("legacy/foodplanner/children");
        //group.MapGet("/GetAllChldrenClasses", );
        //group.MapGet("/GetChildrenByParentToken", );
        //group.MapGet("/GetChildFromChildId/{id}", ); //Katrine
        group.MapGet("/GetParentsByChildId/{childId}", GetGuardiansByCitizenId());
        //group.MapGet("/GetTeachersByChildId/{childId}", );
        //group.MapGet("/GetChildrenByTeacherId/{teacherId}", );

        //group.MapPost("/AddParentToChild/{childId}/parents/{userId}",);
        //group.MapPost("/AddTeacherToChild/{childId}/teachers/{userId}",);

        //group.MapDelete("/RemoveParentFromChild/{childId}/parents/{userId}",);
        //group.MapDelete("/RemoveTeacherFromChild/{childId}/teachers/{userId}",);
    }
  
    private static async Task<List<LegacyUserDTO>> GetGuardiansByCitizenId(NonCitizenService nonCitizenService, UserService userService, int citizenId)  //GetParentByCitizenIdAsync
    {
        var guardianships = await nonCitizenService.GetGuardianshipsByCitizenIdAsync(citizenId); //mangler fra Core2 //return type ResponseGuardianshipsDTO
        List<int> guardianIds = guardianships.NonCitizenIds.ToList();
        List<LegacyUserDTO> guardians = []; //list for storing nonCitizens which are guardian for the user
        foreach (int guardianId in guardianIds) //foreach to get the guardian and not just the ID
        {
            var guardian = await nonCitizenService.GetNonCitizenByIdAsync(guardianId);
            if (guardian != null && guardian.role == LegacyUserRole.Parent) //Role is parent as other caregivers should be returned. Check with foodplanner that this i s correct
            {
                var foodGuardian = new LegacyUserDTO { Id = guardian.Id, FirstName = guardian.FirstName, LastName = guardian.LastName, Email = guardian.Email, Role = LegacyUserRole.Parent, RoleApproved = true };
                guardians.Add(foodGuardian);
            }
        }

        return guardians.Select(g => new LegacyUserDTO //Har ikke et password
        {
            Id = g.Id,
            FirstName = g.FirstName,
            LastName = g.LastName,
            Email = g.Email,
            Role = g.Role,
            RoleApproved = true,
            Archived = false
        }).ToList();
    }


}