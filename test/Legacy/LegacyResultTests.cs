using giraf_core_v2.Legacy.Abstractions;

public class LegacyResultTests
{
    [Fact]
    public void ImplicitValue_Gives200WithValue()
    {
        LegacyResult<string> result = "hello";

        Assert.Equal(200, result.StatusCode);
        Assert.Equal("hello", result.Value);
        Assert.Null(result.ErrorBody);
    }

    [Fact]
    public void ImplicitError_GivesStatusAndBodyWithoutValue()
    {
        var body = new { Message = "nope" };

        LegacyResult<string> result = LegacyResult.BadRequest(body);

        Assert.Equal(400, result.StatusCode);
        Assert.Same(body, result.ErrorBody);
        Assert.Null(result.Value);
    }

    [Fact]
    public void ErrorHelpers_UseTheirStatusCodes()
    {
        Assert.Equal(400, LegacyResult.BadRequest().StatusCode);
        Assert.Equal(401, LegacyResult.Unauthorized().StatusCode);
        Assert.Equal(403, LegacyResult.Forbidden().StatusCode);
        Assert.Equal(404, LegacyResult.NotFound().StatusCode);
        Assert.Equal(409, LegacyResult.Conflict().StatusCode);
        Assert.Equal(500, LegacyResult.Status(500).StatusCode);
    }

    [Fact]
    public void Ok_Gives200()
    {
        Assert.Equal(200, LegacyResult.Ok(5).StatusCode);
        Assert.Equal(5, LegacyResult.Ok(5).Value);
        Assert.Equal(200, LegacyResult.Ok().StatusCode);
    }

    [Fact]
    public void NoContent_Gives204()
    {
        Assert.Equal(204, LegacyResult.NoContent().StatusCode);
    }

    [Fact]
    public void Created_Gives201WithValueAndLocation()
    {
        var result = LegacyResult.Created(42, "/api/Things/42");

        Assert.Equal(201, result.StatusCode);
        Assert.Equal(42, result.Value);
        Assert.Equal("/api/Things/42", result.Location);
    }

    [Fact]
    public void CreatedWithoutValue_Gives201WithoutLocation()
    {
        var result = LegacyResult.Created();

        Assert.Equal(201, result.StatusCode);
        Assert.Null(result.Location);
    }
}