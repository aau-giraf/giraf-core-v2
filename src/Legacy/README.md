# Legacy API adapter layer

This layer lets the existing **VTA** and **Foodplanner** apps use giraf-core-v2 without changing their code. The only change is their base URL. VTA calls `https://<core>/legacy/vta/api/Artefacts` where it used to call `https://<vta-backend>/api/Artefacts`. Everything after the base URL stays the same, and so does the JSON.

The layer only translates between the old API and v2. It's temporary: once both apps call the Core API directly, delete this folder and its lines in `Program.cs`.

## How a request flows

```
VTA / Foodplanner app
        │  legacy URL, legacy JSON
        ▼
Route helper ── binds the request into a record
        │       and builds a LegacyCallContext (who is calling, public base URL)
        ▼
Adapter (one per legacy endpoint) ── calls v2 services ──▶ src/Services
        │                            maps the result with VtaMapper / FoodplannerMapper
        ▼
LegacyResult<T> ── the route helper writes it out exactly as the old backend did
```

## The core idea: one adapter per legacy endpoint

Every legacy endpoint gets one class implementing the same interface:

```csharp
public interface ILegacyAdapter<TRequest, TResponse>
{
    Task<LegacyResult<TResponse>> HandleAsync(
        TRequest request, LegacyCallContext context, CancellationToken ct);
}
```

- **`TRequest`** is a record with everything the endpoint receives (route, query, body), or `NoInput`.
- **`TResponse`** is the legacy DTO the old endpoint returned, `List<T>` of one, `LegacyFile` or `NoBody`.
- **`LegacyResult<T>`** works like `ActionResult<T>` in the old controllers, so ported code reads almost the same.

This is how a future adapter will look. It's illustrative: neither `ImageService.GetAsync` nor `VtaMapper.ToArtefactGetDto` exists yet.

```csharp
// GET /api/Artefacts/{artefactId} (VTA.API ArtefactsController.GetArtefact)
public sealed record GetArtefactRequest([FromRoute] string ArtefactId);

public sealed class GetArtefactAdapter(ImageService images)
    : ILegacyAdapter<GetArtefactRequest, ArtefactGetDTO>
{
    public async Task<LegacyResult<ArtefactGetDTO>> HandleAsync(
        GetArtefactRequest request, LegacyCallContext context, CancellationToken ct)
    {
        if (context.CallerId is null) return LegacyResult.Unauthorized();
        if (!VtaIds.TryToCore(request.ArtefactId, out var id)) return LegacyResult.NotFound();

        var image = await images.GetAsync(id, ct);
        if (image is null) return LegacyResult.NotFound();

        return VtaMapper.ToArtefactGetDto(image, context.PublicBaseUrl);  // 200 + JSON
    }
}
```

The adapter is then connected with one line in `Vta/VtaLegacyRoutes.cs`:

```csharp
vta.MapLegacyGet<GetArtefactRequest, ArtefactGetDTO, GetArtefactAdapter>("/api/Artefacts/{artefactId}");
```

## What's in this folder

| Folder | Contents |
|---|---|
| `Abstractions/` | The shared parts: the adapter interface, `LegacyResult`, the call context, the route helpers and the registration. App-independent |
| `Vta/` | VTA's routes file, caller resolver, `VtaIds` (VTA's string IDs ⇄ v2 int IDs) and `VtaMapper` |
| `Foodplanner/` | The same for Foodplanner. It needs no ID converter, because its IDs are already `int` |

## Rules for adding an adapter

1. **Copy the legacy DTOs verbatim** into `{App}/Dtos/`. Change only the namespace, nothing else: the names and shapes are what the apps depend on.
2. **Create one file per endpoint:** `{App}/{LegacyController}/{LegacyMethod}Adapter.cs`, holding both the request record and the adapter. Start the file with a header comment naming the legacy route and method, like the example above.
3. **Add one route line** to `{App}LegacyRoutes.cs` under its controller's comment, with the path copied exactly from the legacy controller.
4. **Translate only.** Adapters call v2 services, never `AppDbContext`, and never touch `HttpContext`; use `LegacyCallContext` instead. If v2 is missing a feature (boards, meals, ...), build it as a normal service in `src/Services`, so new apps get it too.
5. **Use `List<T>` for collections**, never `IEnumerable<T>`. The implicit conversion to `LegacyResult<T>` doesn't work on interface types.
6. **Never add `giraf_core_v2.Legacy.*` namespaces to `GlobalUsings.cs`.** Both apps have DTOs like `UserDTO`, and v2 has its own `User`, so global usings would make the names ambiguous.

## Open decisions

- **Who calls the Legacy API:** the Flutter apps directly, or the old backends passing calls on.
- **Login and tokens:** v2 has no authentication yet, but VTA expects a JWT with an `id` claim. This lands in each app's caller resolver.
- **Endpoint scope:** every legacy endpoint, or only those the apps use (VTA's Admin, Migration and Sync controllers, for example).
- **v2 models for app-specific data** (artefacts, boards, meals, ingredients, ...).
- **External services and files:** VTA's text-to-speech, Foodplanner's feedback chat, and file storage.
- **Out of scope:** VTA's SignalR `SyncService`, which reads VTA's own database.
