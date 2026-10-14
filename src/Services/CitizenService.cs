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

    public async Task<Citizen?> UpdateCitizenAsync(int citizenId, UpdateCitizenDTO updateCitizen)
    {
        var citizen = await _db.Citizens.FindAsync(citizenId);

		if (citizen is null)
    	{
        	return null;
    	}

		if (updateCitizen.GuardianId is not null) citizen.GuardianId = updateCitizen.GuardianId.Value;
		if (updateCitizen.ClassId is not null) citizen.ClassId = updateCitizen.ClassId.Value;

		await _db.SaveChangesAsync();

		return citizen;	
    }

    public async Task<bool> DeleteCitizenAsync(int citizenId)
    {
		var citizen = await _db.Citizens.FindAsync(citizenId);
		if (citizen is not null)
		{
			_db.Citizens.Remove(citizen);	
			await _db.SaveChangesAsync();	
		}

		return Results.NotFound();
    }
}
