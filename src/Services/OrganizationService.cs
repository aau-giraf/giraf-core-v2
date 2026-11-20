using Microsoft.EntityFrameworkCore;

using Npgsql;

namespace giraf_core_v2.Services;

public enum OrganizationServiceResult
{
    Success,
    OrganizationNotFound,
    ClassNotFound,
    UserNotFound,
    UserRoleAlreadyExists,
    UserRoleNotFound,
    ClassAlreadyExists,
}

public class OrganizationService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<List<OrganizationResponseDTO>> GetAllOrganizationsAsync()
    {
        return await _db.Organizations
            .Select(o => new OrganizationResponseDTO
            {
                OrganizationId = o.Id,
                Name = o.Name,
            })
            .ToListAsync();
    }

    private static bool IsUniqueConstraintViolation(Exception exception)
    {
        return exception.GetBaseException() is PostgresException
        {
            SqlState: PostgresErrorCodes.UniqueViolation
        };
    }

    public async Task<(OrganizationServiceResult, OrganizationResponseDTO?)> GetOrganizationByIdAsync(int organizationId)
    {
        OrganizationResponseDTO? organization = await _db.Organizations
        .Where(o => o.Id == organizationId)
        .Select(o => new OrganizationResponseDTO
        {
            OrganizationId = o.Id,
            Name = o.Name,
        })
        .FirstOrDefaultAsync();

        if (organization is null)
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        return (OrganizationServiceResult.Success, organization);
    }

    public async Task<(OrganizationServiceResult, ClassResponseDTO?)> GetClassInOrganizationAsync(int organizationId, int classId)
    {
        var organizationClass = await _db.Classes
            .Where(c => c.OrganizationId == organizationId && c.Id == classId)
            .Select(c => new ClassResponseDTO
            {
                ClassId = c.Id,
                Name = c.Name,
                OrganizationId = c.OrganizationId,
            })
            .FirstOrDefaultAsync();

        if (organizationClass is not null)
        {
            return (OrganizationServiceResult.Success, organizationClass);
        }

        if (!await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        return (OrganizationServiceResult.ClassNotFound, null);
    }

    public async Task<(OrganizationServiceResult, List<ClassResponseDTO>?)> GetAllClassesInOrganizationAsync(int organizationId)
    {
        // Ensure that the specified organization exists.
        if (!await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        // Fetch the classes and return the list.
        return (OrganizationServiceResult.Success, await _db.Classes
            .Where(c => c.OrganizationId == organizationId)
            .Select(c => new ClassResponseDTO
            {
                ClassId = c.Id,
                Name = c.Name,
                OrganizationId = c.OrganizationId,
            })
            .ToListAsync());
    }

    public async Task<(OrganizationServiceResult, ClassResponseDTO?)> CreateClassInOrganizationAsync(CreateClassDTO createClassDto, int organizationId)
    {
        // Ensure that the specified organization exists.
        if (!await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        Class newClass = new()
        {
            Name = createClassDto.Name.ToLower(),
            OrganizationId = organizationId,
        };

        // TODO: Ask group 16 if we should enforce class name uniqueness.

        _db.Classes.Add(newClass);

        await _db.SaveChangesAsync();

        return (OrganizationServiceResult.Success, new ClassResponseDTO
        {
            ClassId = newClass.Id,
            Name = newClass.Name,
            OrganizationId = newClass.OrganizationId,
        });
    }

    public async Task<(OrganizationServiceResult, OrganizationResponseDTO?)> CreateOrganizationAsync(OrganizationDTO createOrganizationDto)
    {
        var newOrganization = new Organization
        {
            Name = createOrganizationDto.Name.ToLower(),
        };

        _db.Organizations.Add(newOrganization);

        await _db.SaveChangesAsync();

        return (OrganizationServiceResult.Success, new OrganizationResponseDTO
        {
            OrganizationId = newOrganization.Id,
            Name = newOrganization.Name,
        });
    }

    public async Task<(OrganizationServiceResult, UserRoleResponseDTO?)> AddRoleToUserAsync(int organizationId, int nonCitizenId, RoleType role)
    {
        // TODO: Ensure role exists if/when roles are moved to their own table instead of relying on enums.

        // Ensure target user exists.
        if (!await _db.Users.AnyAsync(u => u.Id == nonCitizenId))
        {
            return (OrganizationServiceResult.UserNotFound, null);
        }

        // Ensure that the specified organization exists.
        if (!await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        UserRole newUserRole = new()
        {
            NonCitizenId = nonCitizenId,
            Role = role,
            OrganizationId = organizationId
        };

        try
        {
            _db.UserRoles.Add(newUserRole);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (IsUniqueConstraintViolation(ex))
        {
            return (OrganizationServiceResult.UserRoleAlreadyExists, null);
        }

        return (OrganizationServiceResult.Success, new UserRoleResponseDTO
        {
            NonCitizenId = newUserRole.NonCitizenId,
            Role = newUserRole.Role,
            OrganizationId = newUserRole.OrganizationId
        });
    }

    public async Task<(OrganizationServiceResult, OrganizationResponseDTO?)> UpdateOrganizationAsync(OrganizationDTO updateOrganizationDto, int organizationId)
    {
        // Ensure that the specified organization exists.
        Organization? organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);

        if (organization is null)
        {
            return (OrganizationServiceResult.OrganizationNotFound, null);
        }

        organization.Name = updateOrganizationDto.Name.ToLower();

        await _db.SaveChangesAsync();

        return (OrganizationServiceResult.Success, new OrganizationResponseDTO
        {
            OrganizationId = organization.Id,
            Name = organization.Name,
        });
    }

    public async Task<OrganizationServiceResult> DeleteUserRoleAsync(int organizationId, int nonCitizenId, RoleType role)
    {
        // Ensure that the target user exists.
        if (!await _db.Users.AnyAsync(u => u.Id == nonCitizenId))
        {
            return OrganizationServiceResult.UserNotFound;
        }

        // Ensure that the specified organization exists.
        if (!await _db.Organizations.AnyAsync(o => o.Id == organizationId))
        {
            return OrganizationServiceResult.OrganizationNotFound;
        }

        // Find the user role to remove.
        UserRole? userRole = await _db.UserRoles.FirstOrDefaultAsync(ur =>
            ur.NonCitizenId == nonCitizenId &&
            ur.OrganizationId == organizationId &&
            ur.Role == role);

        if (userRole is null)
        {
            return OrganizationServiceResult.UserRoleNotFound;
        }

        _db.UserRoles.Remove(userRole);
        await _db.SaveChangesAsync();

        return OrganizationServiceResult.Success;
    }

    public async Task<OrganizationServiceResult> DeleteOrganizationAsync(int organizationId)
    {
        Organization? organization = await _db.Organizations.FirstOrDefaultAsync(o => o.Id == organizationId);

        if (organization is null)
        {
            return OrganizationServiceResult.OrganizationNotFound;
        }

        _db.Organizations.Remove(organization);
        await _db.SaveChangesAsync();

        return OrganizationServiceResult.Success;
    }

    public async Task<OrganizationServiceResult> DeleteClassInOrganizationAsync(int organizationId, int classId)
    {
        Class? organizationClass = await _db.Classes.FirstOrDefaultAsync(c => c.OrganizationId == organizationId && c.Id == classId);

        if (organizationClass is null)
        {
            return OrganizationServiceResult.ClassNotFound;
        }

        _db.Classes.Remove(organizationClass);
        await _db.SaveChangesAsync();

        return OrganizationServiceResult.Success;
    }
}