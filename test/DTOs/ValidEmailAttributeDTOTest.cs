using Xunit.Abstractions;

public class ValidEmailAttributeTest(ITestOutputHelper output) : TestBase(output)
{
    [Theory]
    [InlineData("GammelGris@single.dk")]
    [InlineData("Morten@Eelau.com")]
    [InlineData("Tove@Andersen.ai")]
    [InlineData("no-replay@hotmail.dom")]
    [InlineData("numsetrold@lækkermor.dk")]
    [InlineData("LækreMødre@IDitOmråde.dk")]
    [InlineData("Markus@8ruun.dk")]
    public void IsValid_AcceptsValidEmails(string email)
    {
        Assert.True(new ValidEmailAttribute().IsValid(email));
    }

    [Theory]
    [InlineData("NotAnEmail")]
    [InlineData("an@Email.m")]
    [InlineData("()@Email.mk")]
    [InlineData("prut@mk.k")]
    [InlineData("@mail.dk")]
    [InlineData("test@")]
    public void IsValid_RejectsInvalidEmails(string email)
    {
        Assert.False(new ValidEmailAttribute().IsValid(email));
    }

    [Fact]
    public void IsValid_AcceptsNull()
    {
        // null is handled by [Required]
        Assert.True(new ValidEmailAttribute().IsValid(null));
    }
}