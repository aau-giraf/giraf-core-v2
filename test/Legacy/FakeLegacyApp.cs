using System.Text;

using giraf_core_v2.Legacy.Abstractions;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

// Fake caller resolver and adapters for testing the legacy layer without a real legacy app

// The caller sends their ID in the X-Test-User header
public sealed class FakeCallerResolver : ILegacyCallerResolver
{
    public Task<int?> ResolveCallerIdAsync(HttpContext http, CancellationToken ct)
    {
        return Task.FromResult<int?>(int.TryParse(http.Request.Headers["X-Test-User"], out int id) ? id : null);
    }
}

public enum ThingKind { Small, Big }

public sealed record ThingDto(int Id, string DisplayName, ThingKind Kind, int? CallerId, string PublicBaseUrl);

public sealed record GetThingRequest([FromRoute] int ThingId);

// 404 and 400 as IDs trigger the error cases
public sealed class GetThingAdapter : ILegacyAdapter<GetThingRequest, ThingDto>
{
    public Task<LegacyResult<ThingDto>> HandleAsync(GetThingRequest request, LegacyCallContext context, CancellationToken ct)
    {
        LegacyResult<ThingDto> result = request.ThingId switch
        {
            404 => LegacyResult.NotFound(),
            400 => LegacyResult.BadRequest(new { Message = new[] { "Bad thing" } }),
            _ => new ThingDto(request.ThingId, "Thing", ThingKind.Big, context.CallerId, context.PublicBaseUrl),
        };
        return Task.FromResult(result);
    }
}

public sealed class GetTextAdapter : ILegacyAdapter<NoInput, string>
{
    public Task<LegacyResult<string>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<string>>("hello");
    }
}

public sealed record ThingBody(string Name);

public sealed record CreateThingRequest([FromBody] ThingBody Body);

public sealed class CreateThingAdapter : ILegacyAdapter<CreateThingRequest, int>
{
    public Task<LegacyResult<int>> HandleAsync(CreateThingRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult(LegacyResult.Created(request.Body.Name.Length, $"/api/Things/{request.Body.Name.Length}"));
    }
}

public sealed class UpdateThingAdapter : ILegacyAdapter<CreateThingRequest, NoBody>
{
    public Task<LegacyResult<NoBody>> HandleAsync(CreateThingRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult(LegacyResult.Created());
    }
}

public sealed record UploadThingRequest([FromForm] string Name);

public sealed class UploadThingAdapter : ILegacyAdapter<UploadThingRequest, string>
{
    public Task<LegacyResult<string>> HandleAsync(UploadThingRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult<LegacyResult<string>>(request.Name);
    }
}

public sealed class GetFileAdapter : ILegacyAdapter<NoInput, LegacyFile>
{
    public Task<LegacyResult<LegacyFile>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct)
    {
        var content = new MemoryStream(Encoding.UTF8.GetBytes("file content"));
        return Task.FromResult<LegacyResult<LegacyFile>>(new LegacyFile(content, "image/png", "thing.png"));
    }
}

public sealed class DeleteThingAdapter : ILegacyAdapter<GetThingRequest, NoBody>
{
    public Task<LegacyResult<NoBody>> HandleAsync(GetThingRequest request, LegacyCallContext context, CancellationToken ct)
    {
        return Task.FromResult(LegacyResult.NoContent());
    }
}

// Must never be registered
public abstract class AbstractThingAdapter : ILegacyAdapter<NoInput, NoBody>
{
    public abstract Task<LegacyResult<NoBody>> HandleAsync(NoInput request, LegacyCallContext context, CancellationToken ct);
}