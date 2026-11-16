using System.ComponentModel.DataAnnotations;


using Xunit.Abstractions;

public class UpdateUserEmailDTOTest(ITestOutputHelper output) : TestBase(output)
{
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

    [Theory]
    [InlineData("GammelGris@single.dk", "Morten@Eelau.com")]
    [InlineData("Tove@Andersen.ai", "no-replay@hotmail.dom")]
    [InlineData("JegElskerTest@Selvmord.com", "LækreMødre@IDitOmråde.dk")]
    public void Validate_SucceedsWithValidAndDiffentEmail(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO
        {
            OldEmail = oldEmail,
            NewEmail = newEmail,
            Password = "12345678"
        };
        Assert.Empty(dto.Validate(new ValidationContext(dto)));
    }

    [Theory]
    [InlineData("IkkeEnMail", "HellerIkkeEnMail")]
    [InlineData("prut@mk.k", "()@mik.mik")]
    [InlineData("@mail.dk", "test@")]
    public void Validate_FailsWhenMulipleErrorsWhenBothEmailAreInvalid(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO
        {
            OldEmail = oldEmail,
            NewEmail = newEmail,
            Password = "12345678"
        };
        var results = dto.Validate(new ValidationContext(dto)).ToList();
        Assert.Multiple(() =>
            {
                Assert.Equal(2, results.Count);
                Assert.Contains(results, r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.OldEmail)));
                Assert.Contains(results, r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.NewEmail)));
            });
    }

    [Theory]
    [InlineData("IkkeEnMail", "IkkeEnMail")]
    [InlineData("prut@mk.k", "PRUT@MK.K")]
    [InlineData("()@Email.dk", "()@Email.dk")]
    public void Validate_FailsMultipleErrorsWhenEmailsAreInvalidAndEqual(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO
        {
            OldEmail = oldEmail,
            NewEmail = newEmail,
            Password = "12345678"
        };
        var results = dto.Validate(new ValidationContext(dto)).ToList();
        Assert.Multiple(() =>
            {
                Assert.Equal(3, results.Count);
                Assert.Single(results, r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.OldEmail)));
                Assert.Equal(2, results.Count(r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.NewEmail))));
            });
    }
}
