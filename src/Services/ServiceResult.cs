// From WeekPlanner 
public class ServiceResult<T>
{
    public T? Value { get; }
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private ServiceResult(T? value, ServiceError? error)
    {
        Value = value;
        Error = error;
    }

    public static ServiceResult<T> Success(T value) => new(value, null);
    public static ServiceResult<T> Fail(ServiceError error) => new(default, error);
}

/// Non-generic variant for operations that return no value.
public class ServiceResult
{
    public ServiceError? Error { get; }
    public bool IsSuccess => Error is null;

    private ServiceResult(ServiceError? error) => Error = error;

    public static ServiceResult Success() => new(null);
    public static ServiceResult Fail(ServiceError error) => new(error);
}

public record ServiceError(ServiceErrorKind Kind, string Message);

public enum ServiceErrorKind
{
    NotFound,
    Validation,
    Unauthorized,
    Forbidden,
    Conflict,
    Internal
}