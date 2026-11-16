using System.ComponentModel.DataAnnotations;

using giraf_core_v2.DTOs.Images;

using Xunit.Abstractions;

namespace giraf_core_v2.Tests.DTOs;

public class PostImagePromptDTOTest : TestBase
{
    public PostImagePromptDTOTest(ITestOutputHelper output) : base(output) { }
    
    [Theory]
    // Three tests where the owner of the prompt is set to exactly one
    [InlineData(true, false, false)]
    [InlineData(false, true, false)]
    [InlineData(false, false, true)]
    public void Validate_PassesWhenExactlyOneOwnerIsSet(bool citizen, bool class_, bool organization)
    {
        var dto = new PostImagePromptDTO
        {
            Prompt = "a giraffe",
            CitizenId = citizen ? 1 : null,
            ClassId = class_ ? 1 : null,
            OrganizationId = organization ? 1 : null
        };
        // Assert there are no errors and the DTO is valid
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }
    
    [Theory]
    // Five tests that are not valid in the DTO, no owner and multiple owners
    [InlineData(false, false, false)] // no owner
    [InlineData(true, true, false)]
    [InlineData(true, false, true)]
    [InlineData(false, true, true)]
    [InlineData(true, true, true)]
    public void Validate_FailsWhenNotExactlyOneOwnerIsSet(bool citizen, bool class_, bool organization)
    {
        var dto = new PostImagePromptDTO
        {
            Prompt = "a giraffe",
            CitizenId = citizen ? 1 : null,
            ClassId = class_ ? 1 : null,
            OrganizationId = organization ? 1 : null
        };
        // Assert the dto validation has an error
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }
}