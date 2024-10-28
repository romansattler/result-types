using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
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
            AggregateException a => HandleAggregateException(a),
            ValidationException v => HandleValidationException(v),
            KeyNotFoundException v => HandleKeyNotFoundException(v),
            _ => HandleGenericException(exception)
        };

        if (genericArgument is not null)
        {
            var converter = typeof(Result<>).MakeGenericType(genericArgument)
                .GetMethod(nameof(Result.FromEmptyResult), BindingFlags.Public | BindingFlags.Static)
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

    private static Result HandleAggregateException(AggregateException exception)
    {
        return exception.InnerExceptions.All(e => e is ValidationException)
            ? HandleValidationException(exception.InnerExceptions.Cast<ValidationException>().ToArray())
            : HandleGenericException(exception);
    }

    private static Result HandleValidationException(params ValidationException[] exceptions)
    {
        return Result.Invalid(exceptions.Select(e => e.ValidationResult).ToArray());
    }

    private static Result HandleKeyNotFoundException(KeyNotFoundException _)
    {
        return Result.NotFound();
    }

    private static Result HandleGenericException(Exception exception)
    {
        return Result.Error(exception);
    }
}
