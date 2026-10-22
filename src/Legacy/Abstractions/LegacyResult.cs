namespace giraf_core_v2.Legacy.Abstractions;

// What an adapter returns: a success value or an error. Works like ASP.NET's ActionResult<T>
public sealed class LegacyResult<T>
{
    internal LegacyResult(int statusCode, T? value, object? errorBody, string? location)
    {
        StatusCode = statusCode;
        Value = value;
        ErrorBody = errorBody;
        Location = location;
    }

    public int StatusCode { get; }
    public T? Value { get; }            // body on success
    public object? ErrorBody { get; }   // body on failure, written as-is
    public string? Location { get; }    // set only by Created(value, location)

    // 200 with value
    public static implicit operator LegacyResult<T>(T value) => new(200, value, null, null);

    // Error status + optional body
    public static implicit operator LegacyResult<T>(LegacyError error) => new(error.StatusCode, default, error.Body, null);
}

// Non-generic error result, so `return LegacyResult.NotFound();` converts to any LegacyResult<T>
public sealed record LegacyError(int StatusCode, object? Body);

public static class LegacyResult
{
    // Successes
    // 200 + body
    public static LegacyResult<T> Ok<T>(T value) => new(200, value, null, null);

    // 200, no body
    public static LegacyResult<NoBody> Ok() => new(200, new NoBody(), null, null);

    // 204
    public static LegacyResult<NoBody> NoContent() => new(204, new NoBody(), null, null);

    // 201 (legacy CreatedAtAction)
    public static LegacyResult<T> Created<T>(T value, string? location = null) => new(201, value, null, location);

    // 201, no body (legacy Created())
    public static LegacyResult<NoBody> Created() => new(201, new NoBody(), null, null);

    // Errors. The body is optional and written exactly as given
    // 400
    public static LegacyError BadRequest(object? body = null) => new(400, body);

    // 401
    public static LegacyError Unauthorized(object? body = null) => new(401, body);

    // 403
    public static LegacyError Forbidden(object? body = null) => new(403, body);

    // 404
    public static LegacyError NotFound(object? body = null) => new(404, body);

    // 409
    public static LegacyError Conflict(object? body = null) => new(409, body);

    // Anything else, e.g. 500
    public static LegacyError Status(int statusCode, object? body = null) => new(statusCode, body);
}