namespace giraf_core_v2.Services;

public class CitizenService(AppDbContext db)
{
    private readonly AppDbContext _db = db;

    public async Task<Citizen?> GetCitizenAsync(int userId)
    {
        return await _db.Citizens.FindAsync(userId);
    }

    public async Task<Citizen?> CreateCitizenAsync(CreateCitizenDTO createCitizen)
    {
		// 1. Look up the three entities from the IDs in the DTO
    	var user = await _db.Users.FindAsync(createCitizen.UserId);
    	var guardian = await _db.Users.FindAsync(createCitizen.GuardianId);
    	var schoolClass = await _db.Classes.FindAsync(createCitizen.ClassId);

    	// 2. If any of them doesn't exist, stop here
    	if (user is null || guardian is null || schoolClass is null)
      	{
          return null;
      	}
		
		var citizen = new Citizen
		{
			UserId = createCitizen.UserId,
			GuardianId = createCitizen.GuardianId,
			ClassId = createCitizen.ClassId,
		};
		
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
