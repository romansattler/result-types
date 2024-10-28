using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes;

public sealed record class Result : Result<Unit>
{
    public static Result<TValue> Created<TId, TValue>(TId id, TValue value)
    {
        ArgumentNullException.ThrowIfNull(id);
        ArgumentNullException.ThrowIfNull(value);

        return Result<TValue>.Created(id, value);
    }

    public static Result Created<TId>(TId id)
    {
        ArgumentNullException.ThrowIfNull(id);

        return new()
        {
            Status = ResultStatus.Created,
            Id = id
        };
    }

    public static Result<TValue> Ok<TValue>(TValue value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return Result<TValue>.Ok(value);
    }

    public static Result NoContent()
    {
        return new()
        {
            Status = ResultStatus.Ok
        };
    }

    public static Result NotFound(string? message = null)
    {
        return new()
        {
            Status = ResultStatus.NotFound,
            Exception = new NotFoundException(message)
        };
    }

    public static Result Invalid(params ValidationResult[] validationErrors)
    {
        ArgumentNullException.ThrowIfNull(validationErrors);

        if (validationErrors is [])
        {
            throw new ArgumentException("Validation errors must not be empty.", nameof(validationErrors));
        }

        if (validationErrors.Contains(ValidationResult.Success))
        {
            throw new ArgumentException("Validation errors must not contain a success result.", nameof(validationErrors));
        }

        return new()
        {
            Status = ResultStatus.Invalid,
            ValidationErrors = validationErrors
        };
    }

    public static Result Error(Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);

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
