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
        var imageRequest = new PostImageRequestDTO
        {
            TitleEn = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = hasCitizenId ? 1 : null,
            ClassId = hasClassId ? 1 : null,
            OrganizationId = hasOrganizationId ? 1 : null
        };
        Assert.Empty(imageRequest.Validate(new ValidationContext(imageRequest)));
    }

    [Theory]
    [InlineData(false, false, false)]
    [InlineData(true, true, true)]
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    public void Validate_MultipleOwnerIdsAreSet_Fails(bool hasCitizenId, bool hasClassId, bool hasOrganizationId)
    {
        var imageRequest = new PostImageRequestDTO
        {
            TitleEn = "aName",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "timmyTurner.png"),
            CitizenId = hasCitizenId ? 1 : null,
            ClassId = hasClassId ? 1 : null,
            OrganizationId = hasOrganizationId ? 1 : null
        };
        Assert.Single(imageRequest.Validate(new ValidationContext(imageRequest)));
    }

    [Theory]
    [InlineData("jpeg")]
    [InlineData("jpg")]
    [InlineData("png")]
    [InlineData("heic")]
    [InlineData("webp")]
    public void Validate_FileExtensionIsAllowed_Passes(string fileExtension)
    {
        var imageRequest = _imageSeeder.CreateRequestBody(Seed, new(CitizenId: null, ClassId: null, FileExtension: fileExtension));
        Assert.Empty(imageRequest.Validate(new ValidationContext(imageRequest)));
    }

    [Fact]
    public void Validate_FileExtensionIsNotAllowed_Fails()
    {
        var imageRequest = _imageSeeder.CreateRequestBody(Seed, new(CitizenId: null, ClassId: null));
        Assert.Single(imageRequest.Validate(new ValidationContext(imageRequest)));
    }

    [Fact]
    public void Validate_OwnerAndFileAreInvalid_ReturnsMultipleErrors()
    {
        var imageRequest = _imageSeeder.CreateRequestBody(Seed, new());
        var results = imageRequest.Validate(new ValidationContext(imageRequest)).ToList();
        Assert.Multiple(() =>
        {
            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageRequestDTO)));
            Assert.Contains(results, r => r.MemberNames.Contains(nameof(PostImageRequestDTO.File)));
        });
    }
}