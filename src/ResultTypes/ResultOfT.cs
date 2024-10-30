using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.ExceptionServices;

namespace ResultTypes;

public record class Result<TValue>
{
    private protected static Result<TValue> Created<TId>(TId id, TValue value)
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
            Status = ResultStatus.Success,
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
    private ExceptionDispatchInfo? ExceptionDispatchInfo => Exception is not null ? ExceptionDispatchInfo.Capture(Exception) : null;
    public ValidationResult[]? ValidationErrors { get; private protected init; }
    public bool IsSuccess => Status is ResultStatus.Success or ResultStatus.Created;

    [MemberNotNullWhen(true, nameof(Value))]
    public bool HasValue => IsSuccess && Value is not Unit && Value != null;

    public void EnsureSuccess()
    {
        switch (this)
        {
            case { IsSuccess: true }:
                return;

            case { Status: ResultStatus.Invalid, ValidationErrors: not null }:
                throw ValidationErrors.ToAggregateException();

            case { Status: ResultStatus.NotFound }:
                ExceptionDispatchInfo?.Throw();
                throw new NotFoundException(null);

            default:
                ExceptionDispatchInfo?.Throw();
                throw new UnknownErrorException();
        }
    }

    [MemberNotNull(nameof(Value))]
    public void EnsureHasValue()
    {
        EnsureSuccess();

        if (!HasValue)
        {
            throw new MissingValueException();
        }
    }

    public static Result<TValue?> FromUntypedResult(Result result)
    {
        return result switch
        {
            { IsSuccess: true } => new()
            {
                Status = ResultStatus.Success,
                Value = default
            },
            _ => new()
            {
                Status = result.Status,
                Exception = result.Exception,
                ValidationErrors = result.ValidationErrors
            }
        };
    }

    public static implicit operator Result<TValue?>(Result result) => FromUntypedResult(result);
    public static implicit operator Result<TValue>(TValue value) => Result.Success(value);
}
