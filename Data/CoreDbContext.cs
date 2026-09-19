using Microsoft.EntityFrameworkCore;

namespace core.Data {
    public class CoreDbContext : DbContext {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) {

        }
        public DbSet<CitizensService> Citizens {get; set;}
    }
} 

