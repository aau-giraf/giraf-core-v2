using System.ComponentModel.DataAnnotations;

using Xunit.Abstractions;

using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

public class UpdateUserEmailDTOTest(ITestOutputHelper output) : TestBase(output)
{
    // Runs both the attributes and Validate(), like ASP.NET does
    private static List<ValidationResult> ValidateDto(UpdateUserEmailDTO dto)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(dto, new ValidationContext(dto), results, validateAllProperties: true);
        return results;
    }

    [Theory]
    [InlineData("GammelGris@single.dk", "Morten@Eelau.com")]
    [InlineData("Tove@Andersen.ai", "no-replay@hotmail.dom")]
    public void Validate_SucceedsWithValidAndDifferentEmails(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO { OldEmail = oldEmail, NewEmail = newEmail, Password = "12345678" };

        Assert.Empty(ValidateDto(dto));
    }

    [Theory]
    [InlineData("same@test.dk", "same@test.dk")]
    [InlineData("same@test.dk", "SAME@TEST.DK")]
    public void Validate_FailsWhenEmailsAreEqual(string oldEmail, string newEmail)
    {
        var dto = new UpdateUserEmailDTO { OldEmail = oldEmail, NewEmail = newEmail, Password = "12345678" };

        var result = Assert.Single(ValidateDto(dto));
        Assert.Contains(nameof(UpdateUserEmailDTO.NewEmail), result.MemberNames);
    }

    [Fact]
    public void Validate_BothEmailFieldsHaveEmailValidation()
    {
        // Checks that [ValidEmail] is on both properties — the format itself is tested in ValidEmailAttributeTest
        var dto = new UpdateUserEmailDTO { OldEmail = "IkkeEnMail", NewEmail = "HellerIkkeEnMail", Password = "12345678" };

        var results = ValidateDto(dto);

        Assert.Equal(2, results.Count);
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.OldEmail)));
        Assert.Contains(results, r => r.MemberNames.Contains(nameof(UpdateUserEmailDTO.NewEmail)));
    }
}