using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace ResultTypes;

public record class Result<TValue>
{
    private protected static Result<TValue> Created<TId>(TId id, TValue value)
        where TId : struct
    {
        return new()
        {
            Status = ResultStatus.Created,
            Id = id,
            Value = value
        };
    }

    private protected static Result<TValue> Ok(TValue value)
    {
        return new()
        {
            Status = ResultStatus.Ok,
            Value = value
        };
    }

    private protected Result()
    {
    }

    public ResultStatus Status { get; private protected init; }

    public TValue? Value { get; private protected init; }
    public object? Id { get; private protected init; }
    public Exception? Exception { get; private protected init; }
    public ValidationResult[]? ValidationErrors { get; private protected init; }

    [MemberNotNullWhen(true, nameof(Value))]
    public bool IsSuccess => Status is ResultStatus.Ok or ResultStatus.Created;

    [MemberNotNull(nameof(Value))]
    public void AssertSuccess()
    {
        switch (this)
        {
            case { IsSuccess: true }:
                return;
            case { Status: ResultStatus.Invalid, ValidationErrors: not null }:
                throw new AggregateException(ValidationErrors.Select(e => new ValidationException(e, null, null)));
            case { Status: ResultStatus.NotFound }:
                throw Exception ?? new KeyNotFoundException();
            default:
                throw Exception ?? new AggregateException();
        }
    }

    public static Result<TValue?> FromEmptyResult(Result result)
    {
        return result switch
        {
            { IsSuccess: true } => new Result<TValue?>() { Status = ResultStatus.Ok, Value = default },
            _ => new()
            {
                Status = result.Status,
                Exception = result.Exception,
                ValidationErrors = result.ValidationErrors
            }
        };
    }

    public static implicit operator Result<TValue?>(Result result) => FromEmptyResult(result);
    public static implicit operator Result<TValue>(TValue value) => Result.Ok(value);
}
