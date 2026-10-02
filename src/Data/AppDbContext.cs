using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Color> Colors => Set<Color>();

    public DbSet<Organization> Orginazations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserOrganization> UserOrginazations => Set<UserOrganization>();
    
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Class> Classes => Set<Class>();

}