using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Color> Colors => Set<Color>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Orginazations => Set<Organization>();
    public DbSet<UserOrganization> UserOrginazations => Set<UserOrganization>();
}