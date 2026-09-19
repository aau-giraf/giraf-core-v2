using Microsoft.EntityFrameworkCore;

public class CitizensService : ICitizensService {
    private readonly CoreDbContext _context;
    
    public CitizensService(CoreDbContext context) {

    }

}