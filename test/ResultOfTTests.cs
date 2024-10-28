using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes.Tests;

public sealed class ResultOfTTests
{
    [Fact]
    public void EnsureSuccess_ForCreatedResult_WithId_DoesNotThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = Result.Created(id);

        // Act
        result.EnsureSuccess();
    }

    [Fact]
    public void EnsureSuccess_ForCreatedResult_WithIdAdnValue_DoesNotThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new TestValue();
        var result = Result.Created(id, value);

        // Act
        result.EnsureSuccess();
    }

    [Fact]
    public void EnsureSuccess_ForOkResult_DoesNotThrow()
    {
        // Arrange
        var value = new TestValue();
        var result = Result.Ok(value);

        // Act
        result.EnsureSuccess();
    }

    [Fact]
    public void EnsureSuccess_ForNoContentResult_DoesNotThrow()
    {
        // Arrange
        var result = Result.NoContent();

        // Act
        result.EnsureSuccess();
    }

    [Fact]
    public void EnsureSuccess_ForNotFoundResult_ThrowsNotFoundException()
    {
        // Arrange
        var result = Result.NotFound();

        // Act
        void Act() => result.EnsureSuccess();

        // Assert
        Assert.Throws<NotFoundException>(Act);
    }

    [Fact]
    public void EnsureSuccess_ForInvalidResult_ThrowsNotFoundException()
    {
        // Arrange
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var result = Result.Invalid(validationError);

        // Act
        void Act() => result.EnsureSuccess();

        // Assert
        var exception = Assert.Throws<AggregateValidationException>(Act);
        var validationException = Assert.Single(exception.InnerExceptions);
        Assert.Equal(errorMessage, validationException.Message);
    }

    [Fact]
    public void EnsureSuccess_ForErrorResult_ThrowsProvidedException()
    {
        // Arrange
        var expectedException = new TestException();
        var result = Result.Error(expectedException);

        // Act
        void Act() => result.EnsureSuccess();

        // Assert
        var actualException = Assert.Throws<TestException>(Act);
        Assert.Same(expectedException, actualException);
    }

    [Fact]
    public void EnsureHasValue_ForCreatedResult_WithId_DoesNotThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = Result.Created(id);

        // Act
        void Act() => result.EnsureHasValue();

        // Assert
        Assert.Throws<MissingValueException>(Act);
    }

    [Fact]
    public void EnsureHasValue_ForCreatedResult_WithIdAdnValue_DoesNotThrow()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new TestValue();
        var result = Result.Created(id, value);

        // Act
        result.EnsureHasValue();
    }

    [Fact]
    public void EnsureHasValue_ForOkResult_DoesNotThrow()
    {
        // Arrange
        var value = new TestValue();
        var result = Result.Ok(value);

        // Act
        result.EnsureHasValue();
    }

    [Fact]
    public void EnsureHasValue_ForNoContentResult_DoesNotThrow()
    {
        // Arrange
        var result = Result.NoContent();

        // Act
        void Act() => result.EnsureHasValue();

        // Assert
        Assert.Throws<MissingValueException>(Act);
    }

    [Fact]
    public void EnsureHasValue_ForNotFoundResult_ThrowsNotFoundException()
    {
        // Arrange
        var result = Result.NotFound();

        // Act
        void Act() => result.EnsureHasValue();

        // Assert
        Assert.Throws<NotFoundException>(Act);
    }

    [Fact]
    public void EnsureHasValue_ForInvalidResult_ThrowsNotFoundException()
    {
        // Arrange
        var errorMessage = "error message";
        var validationError = new ValidationResult(errorMessage);
        var result = Result.Invalid(validationError);

        // Act
        void Act() => result.EnsureHasValue();

        // Assert
        var exception = Assert.Throws<AggregateValidationException>(Act);
        var validationException = Assert.Single(exception.InnerExceptions);
        Assert.Equal(errorMessage, validationException.Message);
    }

    [Fact]
    public void EnsureHasValue_ForErrorResult_ThrowsProvidedException()
    {
        // Arrange
        var expectedException = new TestException();
        var result = Result.Error(expectedException);

        // Act
        void Act() => result.EnsureHasValue();

        // Assert
        var actualException = Assert.Throws<TestException>(Act);
        Assert.Same(expectedException, actualException);
    }

    [Fact]
    public void FromUntypedResult_ReturnsValidResult()
    {
        // Arrange
        var untypedResult = Result.NoContent();

        // Act
        var result = Result<TestValue>.FromUntypedResult(untypedResult);

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TestValue?>>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Ok, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Null(result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Implicit_FromUntypedResult_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var untypedResult = Result.NoContent();

        // Act
        var result = (Result<TestValue?>)untypedResult;

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TestValue?>>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.False(result.HasValue),
            () => Assert.Equal(ResultStatus.Ok, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Null(result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }

    [Fact]
    public void Implicit_FromValue_StateUnderTest_ExpectedBehavior()
    {
        // Arrange
        var value = new TestValue();

        // Act
        var result = (Result<TestValue>)value;

        // Assert
        Assert.Multiple(
            () => Assert.IsType<Result<TestValue>>(result),
            () => Assert.True(result.IsSuccess),
            () => Assert.True(result.HasValue),
            () => Assert.Equal(ResultStatus.Ok, result.Status),
            () => Assert.Null(result.Id),
            () => Assert.Equal(value, result.Value),
            () => Assert.Null(result.Exception),
            () => Assert.Null(result.ValidationErrors)
        );
    }
}
