namespace BackendBase.Core.Infrastructure;
public sealed class Result<T>
{
    public Result(T value) => Value = value;

    public Result(T value, string successMessage) : this(value) => SuccessMessage = successMessage;

    public Result(ResultStatus status) => Status = status;

    public static implicit operator T(Result<T> result) => result.Value;
    public static implicit operator Result<T>(T value) => new(value);

    public static implicit operator Result<T>(Result result) => new(default(T))
    {
        Status = result.Status,
        Errors = result.Errors,
        SuccessMessage = result.SuccessMessage
    };

    public T Value { get; init; }

    public Type ValueType => typeof(T);

    public ResultStatus Status { get; set; } = ResultStatus.Ok;

    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.NoContent or ResultStatus.Created;

    public string SuccessMessage { get; set; } = string.Empty;

    public IEnumerable<string> Errors { get; set; } = [];

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Success(T value, string successMessage) => new(value, successMessage);

    public static Result<T> Created(T value) => new(ResultStatus.Created) { Value = value };

    public static Result<T> Error(string errorMessage) => new(ResultStatus.Error) { Errors = new[] { errorMessage } };

    public static Result<T> Invalid(IEnumerable<string> errors)
        => new(ResultStatus.Invalid) { Errors = errors };

    public static Result<T> NotFound() => new(ResultStatus.NotFound);

    public static Result<T> NotFound(params string[] errorMessages) => new(ResultStatus.NotFound) { Errors = errorMessages };

    public static Result<T> Unauthorized() => new(ResultStatus.Unauthorized);

    public static Result<T> Unauthorized(params string[] errorMessages) => new(ResultStatus.Unauthorized) { Errors = errorMessages };
}