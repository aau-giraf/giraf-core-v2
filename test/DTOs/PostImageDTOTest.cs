using Xunit.Abstractions;

using System.ComponentModel.DataAnnotations;

using Microsoft.AspNetCore.Http;

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
            Name = "photo",
            File = new FormFile(new MemoryStream([1, 2, 3]), 0, 3, "file", "photo.png"),
            CitizenId = citizen ? 1 : null,
            ClassId = class_ ? 1 : null,
            OrganizationId = organization ? 1 : null
        };
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }
}