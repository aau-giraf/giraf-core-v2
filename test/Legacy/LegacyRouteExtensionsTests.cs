using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

using giraf_core_v2.Legacy.Abstractions;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

// Starts a real web app on a free port with the fake adapters mapped under /legacy/test
public sealed class FakeLegacyAppFixture : IAsyncLifetime
{
    private WebApplication? _app;

    public HttpClient Client { get; private set; } = null!;

    public async Task InitializeAsync()
    {
        var builder = WebApplication.CreateBuilder();
        builder.WebHost.UseUrls("http://127.0.0.1:0");
        builder.Services.AddLegacyAdapters(typeof(FakeLegacyAppFixture).Assembly);

        // Same as Core's Program.cs, so the tests prove legacy responses ignore it
        builder.Services.ConfigureHttpJsonOptions(options =>
            options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        _app = builder.Build();

        var legacy = _app.MapLegacyApp<FakeCallerResolver>("/legacy/test");
        legacy.MapLegacyGet<GetThingRequest, ThingDto, GetThingAdapter>("/api/Things/{thingId:int}");
        legacy.MapLegacyGet<NoInput, string, GetTextAdapter>("/api/Things/Text");
        legacy.MapLegacyGet<NoInput, LegacyFile, GetFileAdapter>("/api/Things/File");
        legacy.MapLegacyPost<CreateThingRequest, int, CreateThingAdapter>("/api/Things/Create");
        legacy.MapLegacyPost<UploadThingRequest, string, UploadThingAdapter>("/api/Things/Upload");
        legacy.MapLegacyPut<CreateThingRequest, NoBody, UpdateThingAdapter>("/api/Things/Update");
        legacy.MapLegacyPatch<CreateThingRequest, NoBody, UpdateThingAdapter>("/api/Things/Patch");
        legacy.MapLegacyDelete<GetThingRequest, NoBody, DeleteThingAdapter>("/api/Things/{thingId:int}");

        await _app.StartAsync();
        Client = new HttpClient { BaseAddress = new Uri(_app.Urls.First()) };
    }

    public async Task DisposeAsync()
    {
        Client.Dispose();
        if (_app is not null)
        {
            await _app.DisposeAsync();
        }
    }
}

public class LegacyRouteExtensionsTests(FakeLegacyAppFixture app) : IClassFixture<FakeLegacyAppFixture>
{
    private readonly HttpClient _client = app.Client;

    [Fact]
    public async Task Get_PassesRouteValueAndCallContextToAdapter()
    {
        var request = new HttpRequestMessage(HttpMethod.Get, "/legacy/test/api/Things/7");
        request.Headers.Add("X-Test-User", "3");

        var response = await _client.SendAsync(request);
        var body = await response.Content.ReadFromJsonAsync<ThingDto>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(7, body!.Id);
        Assert.Equal(3, body.CallerId);
        Assert.Equal($"{_client.BaseAddress!.ToString().TrimEnd('/')}/legacy/test", body.PublicBaseUrl);
    }

    [Fact]
    public async Task Get_WithoutCaller_PassesNullCallerId()
    {
        var body = await _client.GetFromJsonAsync<ThingDto>("/legacy/test/api/Things/7");

        Assert.Null(body!.CallerId);
    }

    [Fact]
    public async Task Get_WritesJsonLikeTheOldControllers()
    {
        var response = await _client.GetAsync("/legacy/test/api/Things/7");
        string json = await response.Content.ReadAsStringAsync();

        // camelCase names and enums as numbers, even though Core's own JSON options write enums as strings
        Assert.Equal("application/json", response.Content.Headers.ContentType!.MediaType);
        Assert.Contains("\"displayName\":\"Thing\"", json);
        Assert.Contains("\"kind\":1", json);
    }

    [Fact]
    public async Task Get_StringValue_IsWrittenAsPlainText()
    {
        var response = await _client.GetAsync("/legacy/test/api/Things/Text");

        Assert.Equal("text/plain", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal("hello", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Get_ErrorWithoutBody_HasEmptyBody()
    {
        var response = await _client.GetAsync("/legacy/test/api/Things/404");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Get_ErrorWithBody_WritesBodyAsJson()
    {
        var response = await _client.GetAsync("/legacy/test/api/Things/400");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("{\"message\":[\"Bad thing\"]}", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Get_File_StreamsContentWithType()
    {
        var response = await _client.GetAsync("/legacy/test/api/Things/File");

        Assert.Equal("image/png", response.Content.Headers.ContentType!.MediaType);
        Assert.Equal("thing.png", response.Content.Headers.ContentDisposition!.FileNameStar);
        Assert.Equal("file content", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Post_BindsJsonBody_AndWritesCreatedWithLocation()
    {
        var response = await _client.PostAsJsonAsync("/legacy/test/api/Things/Create", new { name = "abcd" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("/api/Things/4", response.Headers.Location!.OriginalString);
        Assert.Equal("4", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Post_FormBody_IsNotBlockedByAntiforgery()
    {
        var form = new FormUrlEncodedContent([new KeyValuePair<string, string>("Name", "from form")]);

        var response = await _client.PostAsync("/legacy/test/api/Things/Upload", form);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("from form", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Put_CreatedWithoutValue_Writes201WithEmptyBody()
    {
        var response = await _client.PutAsJsonAsync("/legacy/test/api/Things/Update", new { name = "x" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.Equal("", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Patch_ReachesAdapter()
    {
        var response = await _client.PatchAsJsonAsync("/legacy/test/api/Things/Patch", new { name = "x" });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Delete_NoContent_Writes204()
    {
        var response = await _client.DeleteAsync("/legacy/test/api/Things/7");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}