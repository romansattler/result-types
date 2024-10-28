using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace ResultTypes.Extensions.MediatR;

internal class ResultExceptionHandler<TRequest, TResponse, TException>(ILogger<ResultExceptionHandler<TRequest, TResponse, TException>> logger) : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : notnull
    where TException : Exception
{
    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        if (!IsResultType(typeof(TResponse), out var genericArgument))
        {
            return Task.CompletedTask;
        }

        logger.LogError(exception, "An unhandled error occurred. Converting exception to Result");

        var result = exception switch
        {
            AggregateValidationException a => HandleValidationException(a),
            ValidationException v => HandleValidationException(v),
            NotFoundException v => HandleNotFoundException(v),
            _ => HandleGenericException(exception)
        };

        if (genericArgument is not null)
        {
            var converter = typeof(Result<>).MakeGenericType(genericArgument)
                .GetMethod(nameof(Result.FromUntypedResult), BindingFlags.Public | BindingFlags.Static)
                ?? throw new InvalidOperationException();

            var convertedResult = converter.Invoke(null, [result])!;

            state.SetHandled((TResponse)convertedResult);
        }
        else
        {
            state.SetHandled((TResponse)(object)result);
        }

        return Task.CompletedTask;
    }

    private static bool IsResultType(Type responseType, [NotNullWhen(true)] out Type? genericArgument)
    {
        if (responseType.IsGenericType && responseType.GetGenericArguments() is [Type g])
        {
            genericArgument = g;
            return responseType == typeof(Result<>).MakeGenericType(g);
        }
        else
        {
            genericArgument = null;
            return responseType.IsAssignableTo(typeof(Result));
        }
    }

    private static Result HandleValidationException(AggregateValidationException exception)
    {
        return Result.Invalid(exception.InnerExceptions.Select(e => e.ValidationResult).ToArray());
    }

    private static Result HandleValidationException(ValidationException exception)
    {
        return Result.Invalid(exception.ValidationResult);
    }

    private static Result HandleNotFoundException(NotFoundException exception)
    {
        return Result.NotFound(exception.Message);
    }

    private static Result HandleGenericException(Exception exception)
    {
        return Result.Error(exception);
    }
}
