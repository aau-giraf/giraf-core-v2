using System.ComponentModel.DataAnnotations;


using Xunit.Abstractions;

public class UpdateUserEmailDTOTest : TestBase
{
    public UpdateUserEmailDTOTest(ITestOutputHelper output) : base(output) { }

    [Theory]
    [InlineData("NotAnEmail", "numsetrold@lækkermor.dk")]
    [InlineData("an@Email.m", "Markus@8ruun.dk")]
    [InlineData("()@Email.mk", "Blackman@hotmail.com")]
    public void Validate_FailsWhenNotGivenEmail(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO
        {
            OldEmail = oldEmail,
            NewEmail = newEmail,
            Password = "12345678"
        };
        Assert.Single(dto.Validate(new ValidationContext(dto)));
    }
}
