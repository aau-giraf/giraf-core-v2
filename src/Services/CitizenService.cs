namespace giraf_core_v2.Services;

public class CitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Citizen?> GetCitizenAsync(int citizenId)
    {
        return await _db.Citizens.FindAsync(citizenId);
    }

    public async Task<Citizen> CreateCitizenAsync(int orgId)
    {
        // TODO: link the citizen to orgId once Citizen has an organization relation.
        var citizen = new Citizen();
        _db.Citizens.Add(citizen);
        await _db.SaveChangesAsync();
        return citizen;
    }

    public async Task<Citizen?> UpdateCitizenAsync(int citizenId)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);

        // Return null if citizen does not exist.
        if (citizen is null)
        {
            return null;
        }

        // TODO: apply updated fields once Citizen has properties beyond its id.
        await _db.SaveChangesAsync();
        return citizen;
    }

    public async Task<bool> DeleteCitizenAsync(int citizenId)
    {
        // Return true on successful deletion.
        if (await _db.Citizens.FindAsync(citizenId) is Citizen citizen)
        {
            _db.Citizens.Remove(citizen);
            await _db.SaveChangesAsync();
            return true;
        }

        return false;
    }
}
