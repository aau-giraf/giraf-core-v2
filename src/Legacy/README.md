# Legacy API adapter layer

This folder lets the old **Foodplanner** and **VTA** apps talk to giraf-core-v2 without changing their code. The only change on their side is the base URL:

```
before:  https://<foodplanner-backend>/api/Users/UpdateLoggedIn
after:   https://<core>/legacy/foodplanner/api/Users/UpdateLoggedIn
```

Everything after the base URL stays the same: paths, JSON and status codes. Behind each old endpoint sits a small **adapter**. It translates the old request into calls to v2's services and translates the result back.

It's temporary. Once both apps call the v2 API directly, delete this folder and its lines in `Program.cs`.

## Where everything goes

The finished example is `Foodplanner/Users/UpdateLoggedInAdapter.cs`. Open it next to this README.

```
src/Legacy/
├── Abstractions/                     shared plumbing: you shouldn't need to change anything here
├── Foodplanner/
│   ├── Dtos/                         the old app's DTOs, copied verbatim, one file each
│   │   ├── UserUpdateLoggedInDTO.cs
│   │   └── ...
│   ├── Users/                        one folder per old controller: UsersController → Users/
│   │   ├── UpdateLoggedInAdapter.cs      done: the example to copy
│   │   ├── GetLoggedInAdapter.cs         stub: answers 501 until someone implements it
│   │   └── ...                           one file per endpoint, named {OldMethodName}Adapter.cs
│   ├── FoodplannerLegacyRoutes.cs    one line per endpoint: connects the old URL to its adapter
│   ├── FoodplannerMapper.cs          v2 model ⇄ old DTO conversions that several adapters share
│   └── FoodplannerCallerResolver.cs  works out which v2 user is calling
└── Vta/                              the same for VTA, plus VtaIds (VTA's string IDs ⇄ v2's int IDs)

test/Legacy/Foodplanner/
├── FoodplannerTestApp.cs             starts the real Foodplanner routes for HTTP tests
├── FoodplannerLegacyRoutesTests.cs   checks that every route line exists and can be built
└── Users/
    └── UpdateLoggedInAdapterTests.cs one test file per adapter, in the same folder structure
```

A new controller gets a new folder. For example, `MealsController` becomes `Foodplanner/Meals/` and `test/Legacy/Foodplanner/Meals/`.

**Users endpoints are already stubbed.** Each stub file says at the top what the old endpoint did and what v2 has today. It's already in the route file and answers `501 Not Implemented`. To implement one, open its file and follow steps 3 to 6 below.

## Add an endpoint, step by step

### 1. Find out what the old endpoint did

Read the old code in [foodplanner-api](https://github.com/aau-giraf/foodplanner-api) top-down: the controller (`FoodplannerApi/Controller/`), then the service it calls (`FoodplannerServices/`), then the SQL (`FoodplannerDataAccessSql/`). Write down:

- **Method and path.** The path is `api/{controller name without "Controller"}/{method name}`, for example `PUT api/Users/UpdateLoggedIn`. Use the method name even if a code comment says something else.
- **Inputs.** The `[FromBody]`, `[FromQuery]` and `[FromRoute]` parameters, and whether it needs a logged-in user (`[Authorize]` or a token parameter).
- **Every response.** Each `return` gives a status code and maybe a body. Exceptions the controller doesn't catch were a `500`.

For stubbed endpoints this is done already: it's the comment at the top of the file.

### 2. Copy the DTOs

Copy every DTO the endpoint takes or returns into `Foodplanner/Dtos/`, one file per class. Change **only** the namespace, to `giraf_core_v2.Legacy.Foodplanner.Dtos`, and add a first line saying where it came from. The class names, property names and attributes are what the app depends on, so leave them alone.

All the DTOs the Users controller needs are copied already, including `ErrorResponse`. Check `Dtos/` before you copy one.

### 3. Write the adapter

Create `Foodplanner/{Controller}/{OldMethodName}Adapter.cs`, or open the stub. It holds:

- a **request record** with everything the endpoint receives
- the **adapter**: one class with one method, `HandleAsync`

This is the whole `UpdateLoggedIn` adapter, without its header comment:

```csharp
// The old method took [FromBody] UserUpdateLoggedInDTO user, so the record has one [FromBody] property
public sealed record UpdateLoggedInRequest([FromBody] UserUpdateLoggedInDTO Body);

// <UpdateLoggedInRequest, NoBody>: what comes in, and what goes out (NoBody: the old Created() had no body).
// The constructor asks for the v2 services it needs, here UserService
public sealed class UpdateLoggedInAdapter(UserService users) : ILegacyAdapter<UpdateLoggedInRequest, NoBody>
{
    public async Task<LegacyResult<NoBody>> HandleAsync(
        UpdateLoggedInRequest request, LegacyCallContext context, CancellationToken ct)
    {
        // 1. Who is calling. The old endpoint had [Authorize], so no logged-in user means 401
        if (context.CallerId is not int callerId)
        {
            return LegacyResult.Unauthorized();
        }

        // 2. Translate the old request into what the v2 service expects
        var update = new UpdateUserDTO
        {
            FirstName = request.Body.FirstName,
            LastName = request.Body.LastName,
        };

        // 3. Call v2
        var user = await users.UpdateCurrentUserAsync(callerId, update);

        // 4. Translate the v2 result into the old response
        return user is null ? LegacyResult.NotFound() : LegacyResult.Created();
    }
}
```

Most adapters follow these same four steps. To turn the old controller code into adapter code, use the [translation table](#translating-old-controller-code) below.

You don't register the adapter anywhere: every adapter is registered automatically at startup.

### 4. Add the route line

Add one line to `Foodplanner/FoodplannerLegacyRoutes.cs` under the old controller's comment, with the path copied exactly:

```csharp
foodplanner.MapLegacyPut<Users.UpdateLoggedInRequest, NoBody, Users.UpdateLoggedInAdapter>("/api/Users/UpdateLoggedIn");
//          ^ HTTP method  ^ request record          ^ response ^ adapter                       ^ old path
```

The `Users.` prefix is the folder. It's there because several controllers have methods with the same name, such as `Create`.

For stubs, the line exists already. Only change it if you change the request or response type.

### 5. Test it

Copy `test/Legacy/Foodplanner/Users/UpdateLoggedInAdapterTests.cs` and adapt it. It has two kinds of tests:

- **Adapter tests** call `HandleAsync` directly with a real v2 service on an in-memory database. They cover the behaviour: one test per response the old endpoint could give.
- **HTTP tests** call the real legacy URL through `FoodplannerTestApp`. They prove that the route, the JSON body and the caller header work.

When you implement a stub, also check that its row in `FoodplannerLegacyRoutesTests.cs` still passes. Then run everything:

```
dotnet test
dotnet format --verify-no-changes --include src/Legacy test/Legacy
```

The `--include` limits the format check to the legacy folders. CI runs it without `--include`, on everything.

### 6. Try it by hand

Start Core with `docker compose up --build` and call the endpoint. Until v2 has login, say who you are with the `X-Dev-User-Id` header. On a fresh database, user 1 is the seeded non-citizen Mads Madsen.

```
curl -i -X PUT http://localhost:8080/legacy/foodplanner/api/Users/UpdateLoggedIn \
  -H "X-Dev-User-Id: 1" \
  -H "Content-Type: application/json" \
  -d '{"firstName":"Anna","lastName":"Jensen","email":"anna@example.com"}'
```

Swagger (`http://localhost:8080/swagger`) lists the legacy endpoints too, but it has no field for the `X-Dev-User-Id` header, so use curl or Postman for endpoints that need a caller.

## Translating old controller code

| In the old controller | In the adapter |
|---|---|
| `[Authorize]`, a `token` parameter, `RetrieveIdFromJwtToken(token)` | `context.CallerId`. It's `null` when nobody is logged in: `if (context.CallerId is not int callerId) return LegacyResult.Unauthorized();` |
| `if (!int.TryParse(idString, ...)) return BadRequest(... "Invalid user ID" ...)` | Leave it out. The `CallerId` check above covers it |
| `[FromBody] SomeDTO x` | A request record property: `record XRequest([FromBody] SomeDTO Body)` |
| `[FromQuery] string email` | `record XRequest([FromQuery] string? Email)`. Make it nullable if the old code handled a missing value itself |
| `[FromRoute] int id`, or `{id}` in the route | `record XRequest([FromRoute] int Id)` |
| No parameters besides the token | Use `NoInput` instead of a request record |
| `return Ok(x);` | `return x;` |
| `return Ok(new { EmailExists = exists });` | Make a response record, `record EmailExistsResponse(bool EmailExists)`, and `return new EmailExistsResponse(exists);` |
| `return Created();` | `return LegacyResult.Created();`, with `NoBody` as the response type |
| `return Created(string.Empty, id);` | `return LegacyResult.Created(id);` |
| `return NoContent();` | `return LegacyResult.NoContent();` |
| `return NotFound();` | `return LegacyResult.NotFound();` |
| `return BadRequest(new ErrorResponse { Message = ["..."] });` | `return LegacyResult.BadRequest(new ErrorResponse { Message = ["..."] });` |
| `return BadRequest(ModelState);` | v2 does **not** run the DTO's validation attributes for you; a test pins this. Run them yourself with `Validator.TryValidateObject` and return the errors as `{ "Field": ["message"] }` |
| An exception the controller didn't catch (a `500`) | Return the clean status that fits, usually `404` or `400`. The app can't depend on a crash |
| `[Authorize(Roles = "...")]` | Not ported yet: v2's roles don't match the old ones. Mention the old roles in the header comment |

## Rules

1. **Translate only.** Adapters call v2 services from `src/Services`. They never use `AppDbContext` or `HttpContext`; use `LegacyCallContext` instead. If v2 is missing a feature (pin codes, meals, ...), build it as a normal v2 service, so new apps get it too.
2. **One file per endpoint,** in its old controller's folder. That file holds the request record, the adapter, and a response record if the old endpoint returned an anonymous object. Start the file with the header comment: old route, old method, old behaviour.
3. **Copy DTOs verbatim** into `{App}/Dtos/`. Only the namespace changes.
4. **Use `List<T>` for collections,** never `IEnumerable<T>`. The conversion to `LegacyResult<T>` doesn't work on interfaces.
5. **Never add `giraf_core_v2.Legacy.*` namespaces to `GlobalUsings.cs`.** Foodplanner's DTOs reuse v2 names. Foodplanner's `UserRole` is an enum, while v2's `UserRole` is a class. If one file needs both, add an alias: `using FoodplannerUserRole = giraf_core_v2.Legacy.Foodplanner.Dtos.UserRole;`.
6. **Every request record property says where it comes from:** `[FromBody]`, `[FromQuery]`, `[FromRoute]` or `[FromForm]`. At most one property can be `[FromBody]`.

## How a request flows

```
Foodplanner / VTA app
        │  old URL, old JSON
        ▼
Route line (FoodplannerLegacyRoutes.cs)
        │  binds the request record, asks FoodplannerCallerResolver who is calling
        ▼
Adapter.HandleAsync(request, context)
        │  calls v2 services ──▶ src/Services
        ▼
LegacyResult<T> ── written out exactly like the old backend did (camelCase JSON, enums as numbers, strings as plain text)
```

## Open decisions

- **Login and tokens.** v2 has no login yet. Until it does, the caller is the `X-Dev-User-Id` header, and anyone can claim any ID. When login lands, only the caller resolvers change, not the adapters.
- **Who calls the Legacy API:** the Flutter apps directly, or the old backends passing calls on.
- **Email.** `UpdateLoggedIn` ignores the email for now, because v2's update doesn't save it and citizens have no email in v2. The old app still gets `201`.
- **Passwords.** The old `UpdatePassword` sets a new password without the old one, but v2 requires the old one.
- **Pin codes.** v2 users have no pin code. `UpdatePinCode`, `CheckPinCode` and `HasPinCode` need a field and a migration first.
- **Roles.** How the old roles (Admin, Child, Teacher, Parent) map to v2's citizens and organization roles.
- **Endpoint scope:** every legacy endpoint, or only those the apps use.
- **v2 models for app-specific data** (meals, ingredients, artefacts, boards, ...), and **external services** (VTA's text-to-speech, Foodplanner's feedback chat, file storage).
- **Out of scope:** VTA's SignalR `SyncService`, which reads VTA's own database.
