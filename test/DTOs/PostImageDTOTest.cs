using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

using Xunit.Abstractions;

public class PostImageDTOTest : TestBase
{
    public PostImageDTOTest(ITestOutputHelper output) : base(output) { }

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Validate_PassesWhenAnyOwnerIdIsSet(bool citizen, bool class_, bool organization)
    {
        var dto = new PostImageDTO
        {
            Name = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = citizen ? 1 : null,
            ClassId = class_ ? 1 : null,
            OrganizationId = organization ? 1 : null
        };
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public void Validate_FailsByPassingMultipleIdentifiers(bool citizen, bool class_, bool organization)
    {
        var dto = new PostImageDTO
        {
            Name = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = citizen ? 1 : null,
            ClassId = class_ ? 1 : null,
            OrganizationId = organization ? 1 : null
        };
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData("jpeg")]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("heic")]
    [InlineData("webp")]
    public void Validate_PassesWithCorrectFileExt(string ext)
    {
        var dto = ImageSeeder.CreateRequestBody(Seed, citizenId: null, classId: null, ext: ext);
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Fact]
    public void Validate_FailsWithIncorrectFileExt()
    {
        var dto = ImageSeeder.CreateRequestBody(Seed, citizenId: null, classId: null);
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }

    [Fact]
    public void Validate_FailsMultipleErrorsInDTO()
    {
        var dto = ImageSeeder.CreateRequestBody(Seed);
        var results = dto.Validate(new ValidationContext(dto)).ToList();
        Assert.Multiple(() =>
        {
            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageDTO)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageDTO.File)));
        });
    }
}