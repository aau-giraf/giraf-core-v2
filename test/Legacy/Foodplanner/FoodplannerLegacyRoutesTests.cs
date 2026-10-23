using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

// Checks the route lines in FoodplannerLegacyRoutes.cs: every endpoint exists with the old path and HTTP method.
// Reading the endpoints also makes ASP.NET Core build them, so a request record it can't bind fails here, not at runtime
public class FoodplannerLegacyRoutesTests(FoodplannerTestApp app) : IClassFixture<FoodplannerTestApp>
{
    [Theory]
    [InlineData("POST", "/api/Users/Create")]
    [InlineData("POST", "/api/Users/CreateUserChildren")]
    [InlineData("POST", "/api/Users/Login")]
    [InlineData("POST", "/api/Users/LoginChild")]
    [InlineData("PUT", "/api/Users/UpdatePinCode")]
    [InlineData("POST", "/api/Users/CheckPinCode")]
    [InlineData("GET", "/api/Users/EmailExists")]
    [InlineData("GET", "/api/Users/HasPinCode")]
    [InlineData("GET", "/api/Users/GetLoggedIn")]
    [InlineData("PUT", "/api/Users/UpdateLoggedIn")]
    [InlineData("PUT", "/api/Users/UpdatePassword")]
    [InlineData("DELETE", "/api/Users/DeleteLoggedInUser")]
    public void UsersController_EndpointIsMapped(string method, string path)
    {
        var endpoints = app.App.Services.GetRequiredService<EndpointDataSource>().Endpoints.OfType<RouteEndpoint>();

        Assert.Contains(endpoints, endpoint =>
            endpoint.RoutePattern.RawText == "/legacy/foodplanner" + path
            && endpoint.Metadata.GetMetadata<HttpMethodMetadata>()?.HttpMethods.Contains(method) == true);
    }
}