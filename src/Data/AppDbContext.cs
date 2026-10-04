using Microsoft.EntityFrameworkCore;

namespace giraf_core_v2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Color> Colors => Set<Color>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
}
