using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //Singular table names from model class, instead of plural from DbSet variables below 
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
    }

    public DbSet<Color> Colors => Set<Color>();

    public DbSet<Organization> Orginazations => Set<Organization>();
    public DbSet<User> Users => Set<User>();
    public DbSet<UserOrganization> UserOrginazations => Set<UserOrganization>();
    
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

}