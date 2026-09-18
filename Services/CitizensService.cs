using Microsoft.EntityFrameworkCore;

public class CitizensService : ICitizensService {
    private readonly CoreDbContext context;
    
    public CitizensService(CoreDbContext context) {

    }

}