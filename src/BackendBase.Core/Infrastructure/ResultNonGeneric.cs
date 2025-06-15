namespace BackendBase.Core.Infrastructure;
public class Result
{
    protected Result() { }

    public Result(ResultStatus status) => Status = status;

    public Result(string errorMessage) : this(ResultStatus.Error)
        => Errors = new[] { errorMessage };

    public ResultStatus Status { get; set; } = ResultStatus.Ok;

    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.NoContent or ResultStatus.Created;

    public string SuccessMessage { get; set; } = string.Empty;

    public IEnumerable<string> Errors { get; set; } = [];

    public static Result Success() => new();

    public static Result Success(string message) => new() { SuccessMessage = message };

    public static Result Created() => new(ResultStatus.Created);

    public static Result Error(string errorMessage) => new(errorMessage);

    public static Result Invalid(IEnumerable<string> errors)
        => new(ResultStatus.Invalid) { Errors = errors };

    public static Result NotFound() => new(ResultStatus.NotFound);

    public static Result NotFound(params string[] errorMessages)
        => new(ResultStatus.NotFound) { Errors = errorMessages };

    public static Result Unauthorized() => new(ResultStatus.Unauthorized);

    public static Result Unauthorized(params string[] errorMessages)
        => new(ResultStatus.Unauthorized) { Errors = errorMessages };
}