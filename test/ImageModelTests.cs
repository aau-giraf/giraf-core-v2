using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

namespace giraf_core_v2.Tests;

// Verifies the database mapping of the Image model. Uses the Npgsql provider's design-time model (no connection is opened), since check constraints, defaults and column types are relational metadata the InMemory provider does not have
public class ImageModelTests
{
    private readonly IEntityType _image;

    public ImageModelTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=unused")
            .Options;

        using var context = new AppDbContext(options);
        _image = context.GetService<IDesignTimeModel>().Model.FindEntityType(typeof(Image))!;
    }

    [Fact]
    public void ImageIsMappedToSingularTable() =>
        Assert.Equal("Image", _image.GetTableName());

    [Fact]
    public void IdIsUuidPrimaryKey()
    {
        var key = _image.FindPrimaryKey()!;
        Assert.Equal(nameof(Image.Id), Assert.Single(key.Properties).Name);
        Assert.Equal("uuid", _image.FindProperty(nameof(Image.Id))!.GetColumnType());
    }

    [Theory]
    [InlineData(nameof(Image.StorageKey), 512)]
    [InlineData(nameof(Image.FileName), 255)]
    [InlineData(nameof(Image.ContentType), 100)]
    public void StringColumnsHaveMaxLength(string property, int maxLength)
    {
        var prop = _image.FindProperty(property)!;
        Assert.Equal(maxLength, prop.GetMaxLength());
        Assert.False(prop.IsNullable);
    }

    [Fact]
    public void StorageKeyIsUnique()
    {
        var index = _image.GetIndexes().Single(i => i.Properties.Select(p => p.Name).SequenceEqual([nameof(Image.StorageKey)]));
        Assert.True(index.IsUnique);
    }

    [Fact]
    public void CreatedAtDefaultsToNow()
    {
        var prop = _image.FindProperty(nameof(Image.CreatedAt))!;
        Assert.Equal("now()", prop.GetDefaultValueSql());
        Assert.Equal("timestamp with time zone", prop.GetColumnType());
    }

    [Theory]
    [InlineData("CK_Image_SizeBytes_Positive", "\"SizeBytes\" > 0")]
    [InlineData("CK_Image_ContentType_Image", "\"ContentType\" LIKE 'image/%'")]
    public void CheckConstraintsAreDefined(string name, string sql)
    {
        var constraint = _image.GetCheckConstraints().Single(c => c.ModelName == name);
        Assert.Equal(sql, constraint.Sql);
    }

    [Theory]
    [InlineData(nameof(Image.UserId), typeof(User), true)]
    [InlineData(nameof(Image.OrganizationId), typeof(Organization), false)]
    [InlineData(nameof(Image.CitizenId), typeof(Citizen), false)]
    public void ForeignKeysRestrictDelete(string property, Type principal, bool required)
    {
        var fk = _image.GetForeignKeys().Single(f => f.Properties.Single().Name == property);
        Assert.Equal(principal, fk.PrincipalEntityType.ClrType);
        Assert.Equal(required, fk.IsRequired);
        Assert.Equal(DeleteBehavior.Restrict, fk.DeleteBehavior);
    }

    [Fact]
    public void CitizenForeignKeyTargetsCitizenUserId()
    {
        var fk = _image.GetForeignKeys().Single(f => f.Properties.Single().Name == nameof(Image.CitizenId));
        Assert.Equal(nameof(Citizen.UserId), fk.PrincipalKey.Properties.Single().Name);
    }
}
