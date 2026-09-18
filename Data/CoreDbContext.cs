using Microsoft.EntityFrameworkCore;

namespace giraf-core-v2.Data {
    public class CoreDbContext : DbContext {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) {

        }
        public DbSet<Citizens> Citizens {get; set;}
        public DbSet<auth_group> auth_group {get; set;}
    }
} 