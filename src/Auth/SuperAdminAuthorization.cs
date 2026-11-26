using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;


// https://stackoverflow.com/questions/60669157/requireauthorization-on-endpoints-mapcontrollerroute-not-working-and-every-r
// RoleHandler - Enum burde gøre det i UserRole model og en rolehandler skal ikke bruges - Så slet SuperAdminHandler

public static class Policies
{
    public const string SuperAdmin = "SuperAdmin";
}

public class SuperAdminRequirement : IAuthorizationRequirement { }

public class SuperAdminHandler(AppDbContext db) : AuthorizationHandler <SuperAdminRequirement>
{
    private readonly AppDbContext _db = db;

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SuperAdminRequirement requirement)
    {
        var idClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? context.User.FindFirst("sub")?.Value;

        if (!int.TryParse(idClaim, out var nonCitizenId))
        {
            return; // Gives a 403, when not found
        }

        // SuperAdmin is global, so we dont care about the columm in the database
        bool isSuperAdmin = await _db.UserRoles
            .AsNoTracking()
            .AnyAsync(ur => ur.NonCitizenId == nonCitizenId && ur.Role == RoleType.SuperAdmin);
    }
}

