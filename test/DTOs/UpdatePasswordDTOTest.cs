using System.ComponentModel.DataAnnotations;

using Xunit.Abstractions;

public class UpdatePasswordDTOTest(ITestOutputHelper output) : TestBase(output)
{
    [Theory]
    [InlineData("12345678", "87654321")]
    [InlineData("Gammeltpassword", "nytpassword")]
    [InlineData("Password1", "password1")]
    public void Validate_PassesWhenPasswordsAreDifferent(string oldPassword, string newPassword)
    {
        var dto = new UpdatePasswordDTO
        {
            OldPassword = oldPassword,
            NewPassword = newPassword
        };
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData("KnapSåHemmeligt1234567890")]
    [InlineData("Superhemmeligt:)123")]
    [InlineData("æøå!#%&/()")]
    public void Validate_FailsWhenPasswordsAreEqual(string password)
    {
        var dto = new UpdatePasswordDTO
        {
            OldPassword = password,
            NewPassword = password
        };
        var result = Assert.Single(dto.Validate(new ValidationContext(dto)));
        Assert.Contains(nameof(UpdatePasswordDTO.NewPassword), result.MemberNames);
    }
}