using System.ComponentModel.DataAnnotations;

namespace giraf_core_v2.Tests.DTOs;

public class PostImageRequestDTOTest(ITestOutputHelper output) : TestBase(output)
{
    private readonly ImageSeeder _imageSeeder = new();

    [Theory]
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Validate_OneOwnerIdIsSet_Passes(bool hasCitizenId, bool hasClassId, bool hasOrganizationId)
    {
        var dto = new PostImageRequestDTO
        {
            Name = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = hasCitizenId ? 1 : null,
            ClassId = hasClassId ? 1 : null,
            OrganizationId = hasOrganizationId ? 1 : null
        };
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public void Validate_MultipleOwnerIdsAreSet_Fails(bool hasCitizenId, bool hasClassId, bool hasOrganizationId)
    {
        var dto = new PostImageRequestDTO
        {
            Name = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = hasCitizenId ? 1 : null,
            ClassId = hasClassId ? 1 : null,
            OrganizationId = hasOrganizationId ? 1 : null
        };
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData("jpeg")]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("heic")]
    [InlineData("webp")]
    public void Validate_FileExtensionIsAllowed_Passes(string fileExtension)
    {
        var dto = _imageSeeder.CreateRequestBody(Seed, new(CitizenId: null, ClassId: null, FileExtension: fileExtension));
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Fact]
    public void Validate_FileExtensionIsNotAllowed_Fails()
    {
        var dto = _imageSeeder.CreateRequestBody(Seed, new(CitizenId: null, ClassId: null));
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }

    [Fact]
    public void Validate_OwnerAndFileAreInvalid_ReturnsMultipleErrors()
    {
        var dto = _imageSeeder.CreateRequestBody(Seed, new());
        var results = dto.Validate(new ValidationContext(dto)).ToList();
        Assert.Multiple(() =>
        {
            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageRequestDTO)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageRequestDTO.File)));
        });
    }
}