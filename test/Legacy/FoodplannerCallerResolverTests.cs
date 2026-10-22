using giraf_core_v2.Legacy.Foodplanner;

using Microsoft.AspNetCore.Http;

public class FoodplannerCallerResolverTests
{
    [Fact]
    public async Task ResolveCallerIdAsync_ReadsDevUserIdHeader()
    {
        var http = new DefaultHttpContext();
        http.Request.Headers["X-Dev-User-Id"] = "5";

        int? callerId = await new FoodplannerCallerResolver().ResolveCallerIdAsync(http, CancellationToken.None);

        Assert.Equal(5, callerId);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("-3")]
    public async Task ResolveCallerIdAsync_MissingOrInvalidHeader_GivesNull(string? header)
    {
        var http = new DefaultHttpContext();
        if (header is not null)
        {
            http.Request.Headers["X-Dev-User-Id"] = header;
        }

        int? callerId = await new FoodplannerCallerResolver().ResolveCallerIdAsync(http, CancellationToken.None);

        Assert.Null(callerId);
    }
}