using System.ComponentModel.DataAnnotations;

namespace ResultTypes;

public sealed record class Result : Result<Unit>
{
    public static Result<TValue> Created<TId, TValue>(TId id, TValue value)
        where TId : struct
    {
        return Result<TValue>.Created(id, value);
    }

    public static Result Created<TId>(TId id)
        where TId : struct
    {
        return new()
        {
            Status = ResultStatus.Created,
            Id = id
        };
    }

    public static Result<TValue> Ok<TValue>(TValue value)
    {
        return Result<TValue>.Ok(value);
    }

    public static Result NoContent()
    {
        return new()
        {
            Status = ResultStatus.Ok
        };
    }

    public static Result NotFound()
    {
        return new()
        {
            Status = ResultStatus.NotFound
        };
    }

    public static Result Invalid(params ValidationResult[] validationErrors)
    {
        return new()
        {
            Status = ResultStatus.Invalid,
            ValidationErrors = validationErrors
        };
    }

    public static Result Error(Exception exception)
    {
        return new()
        {
            Status = ResultStatus.Error,
            Exception = exception
        };
    }

    private Result()
    {
        Value = Unit.Instance;
    }
}
