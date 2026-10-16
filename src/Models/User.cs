using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace giraf_core_v2.Models;

[Index(nameof(Username), IsUnique = true)] //To enforce UNIQUE constraint
public abstract class User
{
    [Key]
    public int Id { get; init; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public required string Username { get; set; }
    public required string Password { get; set; }
}

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        /* TPT (Table-per-Type): all types in the User hierarchy mapped to individual db tables, joined by PK/FK Id
        Allows polymorphism in user handling, maintaining shared constraints - e.g. UNIQUE Username.*/
        builder.UseTptMappingStrategy();
    }
}