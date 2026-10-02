using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using giraf_core_v2.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Color> Colors => Set<Color>();

    public DbSet<User> Users => Set<User>();
    public DbSet<Organization> Organizations => Set<Organization>();
    public DbSet<UserOrganization> UserOrganizations => Set<UserOrganization>();
    public DbSet<Citizen> Citizens => Set<Citizen>();
    public DbSet<Class> Classes => Set<Class>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Discovers & applies all explicit database-model mapping configurations, which implement IEntityTypeConfiguration<Model>.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        //Singular table names from model class, instead of plural from DbSet variables below 
        configurationBuilder.Conventions.Remove(typeof(TableNameFromDbSetConvention));
    }
}