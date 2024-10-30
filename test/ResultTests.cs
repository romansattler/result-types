using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes.Tests;

public sealed class ResultTests
{
    [Fact]
    public void Created_ForValidIdAndValue_ReturnsValidResult()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new TestValue();

        // Act
        var result = Result.Created(id, value);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TestValue>>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.True(result.HasValue),
            () => Assert.Equal(ResultStatus.Created, result.Status),
            () => Assert.Equal(id, result.Id),
            () => Assert.Equal(value, result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Created_WithNullId_ThrowsArgumentNullException()
    {
        // Act
        static Result<TestValue> Act() => Result.Created<string, TestValue>(null!, new TestValue());

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Created_WithNullValue_ThrowsArgumentNullException()
    {
        // Act
        static Result<TestValue> Act() => Result.Created<string, TestValue>("id", null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Created_ForValidIdWithoutValue_ReturnsValidResult()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var result = Result.Created(id);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Created, result.Status),
            () => Assert.Equal(id, result.Id),
            () => Assert.Equal(Unit.Instance, result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Created_WithNullIdWithoutValue_ThrowsArgumentNullException()
    {
        // Act
        static Result Act() => Result.Created<string>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Success_ForValidValue_ReturnsValidResult()
    {
        // Arrange
        var value = new TestValue();

        // Act
        var result = Result.Success(value);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TestValue>>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.True(result.HasValue),
            () => Assert.Equal(ResultStatus.Success, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(value, result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Success_WithNullValue_ThrowsArgumentNullException()
    {
        // Act
        static Result<TestValue> Act() => Result.Success<TestValue>(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Success_ReturnValidResult()
    {
        // Act
        var result = Result.Success();

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Success, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(Unit.Instance, result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void NotFound_WithMessage_ReturnsValidResult()
    {
        // Arrange
        var message = "message";

        // Act
        var result = Result.NotFound(message);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result>(result),
            () => Assert.False(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.NotFound, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(Unit.Instance, result.Value),
            () => Assert.IsType<NotFoundException>(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Invalid_WithValidationError_ReturnsValidResult()
    {
        // Arrange
        var validationError = new ValidationResult("error message");

        // Act
        var result = Result.Invalid(validationError);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result>(result),
            () => Assert.False(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Invalid, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(Unit.Instance, result.Value),
            () => Assert.Null(result.Exception),
            () =>
            {
                Assert.NotNull(result.ValidationErrors);
                Assert.Collection(result.ValidationErrors, e => Assert.Equal(validationError, e));
            });
    }

    [Fact]
    public void Invalid_WithNullAsError_ThrowsArgumentException()
    {
        // Act
        static Result Act() => Result.Invalid(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void Invalid_WithSuccessAsError_ThrowsArgumentException()
    {
        // Act
        static Result Act() => Result.Invalid(ValidationResult.Success!);

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Invalid_WithNoError_ThrowsArgumentException()
    {
        // Act
        static Result Act() => Result.Invalid();

        // Assert
        Assert.Throws<ArgumentException>(Act);
    }

    [Fact]
    public void Error_WithException_ReturnsValidResult()
    {
        // Arrange
        var exception = new Exception();

        // Act
        var result = Result.Error(exception);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result>(result),
            () => Assert.False(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Error, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(Unit.Instance, result.Value),
            () => Assert.Equal(exception, result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Error_WithNullAsException_ThrowsArgumentNullException()
    {
        // Act
        static Result Act() => Result.Error(null!);

        // Assert
        Assert.Throws<ArgumentNullException>(Act);
    }
}
