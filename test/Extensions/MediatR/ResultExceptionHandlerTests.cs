using MediatR.Pipeline;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using ResultTypes.Exceptions;
using ResultTypes.Extensions.MediatR;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes.Tests.Extensions.MediatR;

public class ResultExceptionHandlerTests
{
    [Fact]
    public async Task Handle_ForNonResult_DoesNotSetHandled()
    {
        // Arrange
        var request = new NonResultTestRequest();
        var exception = new Exception();

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<NonResultTestRequest, object, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<NonResultTestRequest, object, Exception>(logger);

        var state = new RequestExceptionHandlerState<object>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.False(state.Handled);
    }

    [Fact]
    public async Task Handle_ForUnitResultAndGenericException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new UnitResultTestRequest();
        var exception = new Exception();

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<UnitResultTestRequest, Result, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<UnitResultTestRequest, Result, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result>(state.Response);
                Assert.Equal(ResultStatus.Error, response.Status);
            }
        );
    }

    [Fact]
    public async Task Handle_ForTestValueResultAndGenericException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new ValueResultTestRequest();
        var exception = new Exception();

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result<TestValue>>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result<TestValue>>(state.Response);
                Assert.Equal(ResultStatus.Error, response.Status);
            }
        );
    }

    [Fact]
    public async Task Handle_ForUnitResultValidationException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new UnitResultTestRequest();
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var exception = new ValidationException(validationError, null, null);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<UnitResultTestRequest, Result, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<UnitResultTestRequest, Result, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.Invalid, response.Status),
                    () =>
                    {
                        Assert.NotNull(response.ValidationErrors);
                        Assert.Collection(response.ValidationErrors, e => Assert.Same(validationError, e));
                    }
                );
            }
        );
    }

    [Fact]
    public async Task Handle_ForTestValueResultValidationException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new ValueResultTestRequest();
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var exception = new ValidationException(validationError, null, null);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result<TestValue>>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result<TestValue>>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.Invalid, response.Status),
                    () =>
                    {
                        Assert.NotNull(response.ValidationErrors);
                        Assert.Collection(response.ValidationErrors, e => Assert.Same(validationError, e));
                    }
                );
            }
        );
    }

    [Fact]
    public async Task Handle_ForUnitResultAggregateValidationException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new UnitResultTestRequest();
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var validationException = new ValidationException(validationError, null, null);
        var exception = new AggregateValidationException([validationException]);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<UnitResultTestRequest, Result, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<UnitResultTestRequest, Result, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.Invalid, response.Status),
                    () =>
                    {
                        Assert.NotNull(response.ValidationErrors);
                        Assert.Collection(response.ValidationErrors, e => Assert.Same(validationError, e));
                    }
                );
            }
        );
    }

    [Fact]
    public async Task Handle_ForTestValueResultAggregateValidationException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new ValueResultTestRequest();
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var validationException = new ValidationException(validationError, null, null);
        var exception = new AggregateValidationException([validationException]);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result<TestValue>>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result<TestValue>>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.Invalid, response.Status),
                    () =>
                    {
                        Assert.NotNull(response.ValidationErrors);
                        Assert.Collection(response.ValidationErrors, e => Assert.Same(validationError, e));
                    }
                );
            }
        );
    }

    [Fact]
    public async Task Handle_ForUnitResultNotFoundException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new UnitResultTestRequest();
        var message = "error message";
        var exception = new NotFoundException(message);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<UnitResultTestRequest, Result, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<UnitResultTestRequest, Result, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.NotFound, response.Status),
                    () => Assert.Equal(message, response.Exception?.Message)
                );
            }
        );
    }

    [Fact]
    public async Task Handle_ForTestValueResultNotFoundException_SetsHandledAndCorrectResponse()
    {
        // Arrange
        var request = new ValueResultTestRequest();
        var message = "error message";
        var exception = new NotFoundException(message);

        var logger = NullLoggerFactory.Instance.CreateLogger<ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>>();
        var resultExceptionHandler = new ResultExceptionHandler<ValueResultTestRequest, Result<TestValue>, Exception>(logger);

        var state = new RequestExceptionHandlerState<Result<TestValue>>();

        // Act
        await resultExceptionHandler.Handle(request, exception, state, default);

        // Assert
        Assert.Multiple(
            () => Assert.True(state.Handled),
            () =>
            {
                var response = Assert.IsType<Result<TestValue>>(state.Response);

                Assert.Multiple(
                    () => Assert.Equal(ResultStatus.NotFound, response.Status),
                    () => Assert.Equal(message, response.Exception?.Message)
                );
            }
        );
    }
}
