using Microsoft.AspNetCore.Http;
using ResultTypes.Extensions.AspNetCore;

namespace ResultTypes.Tests.Extensions.AspNetCore;

public class ResultTypeOfTTests
{
    [Fact]
    public async Task OkResultType_WithValue_ReturnsValidResultType()
    {
        // Arrange
        var value = new TestValue();
        var result = Result.Success(value);

        // Act
        var resultType = new ResultType.Ok<TestValue>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode),
            () => Assert.Equal(value, resultType.Value),
            () => Assert.Equal(value, (resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task OkResultType_WithoutValue_ReturnsValidResultType()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var resultType = new ResultType.Ok<Unit>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status204NoContent, resultType.StatusCode),
            () => Assert.Null(resultType.Value),
            () => Assert.Null((resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task NoContentResultType_WithoutValue_ReturnsValidResultType()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var resultType = new ResultType.NoContent(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status204NoContent, resultType.StatusCode),
            () => Assert.Null(resultType.Value),
            () => Assert.Null((resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task CreatedResultType_WithValue_ReturnsValidResultType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new TestValue();
        var result = Result.Created(id, value);

        // Act
        var resultType = new ResultType.Created<TestValue>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status201Created, resultType.StatusCode),
            () => Assert.Equal(value, resultType.Value),
            () => Assert.Equal(value, (resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task CreatedResultType_WithoutValue_ReturnsValidResultType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = Result.Created(id);

        // Act
        var resultType = new ResultType.Created<Unit>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status201Created, resultType.StatusCode),
            () => Assert.Null(resultType.Value),
            () => Assert.Null((resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task CollectionResultType_WithValue_ReturnsValidResultType()
    {
        // Arrange
        IEnumerable<TestValue> values = [new(), new()];
        var result = Result.Success(values);

        // Act
        var resultType = new ResultType.Collection<TestValue>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode),
            () => Assert.Equal(values, resultType.Value),
            () => Assert.Equal(values, (resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task CollectionResultType_WithEmptyValue_ReturnsValidResultType()
    {
        // Arrange
        IEnumerable<TestValue> values = [];
        var result = Result.Success(values);

        // Act
        var resultType = new ResultType.Collection<TestValue>(result);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status200OK, resultType.StatusCode),
            () => Assert.Equal(values, resultType.Value),
            () => Assert.Equal(values, (resultType as IValueHttpResult).Value)
        );
    }

    [Fact]
    public async Task CollectionResultType_WithoutValue_ReturnsValidResultType()
    {
        // Arrange
        var result = (Result<IEnumerable<TestValue>?>)Result.Success();

        // Act
        var resultType = new ResultType.Collection<TestValue>(result!);

        // Assert
        Assert.Multiple(
            () => Assert.Equal(StatusCodes.Status204NoContent, resultType.StatusCode),
            () => Assert.Null(resultType.Value),
            () => Assert.Null((resultType as IValueHttpResult).Value)
        );
    }
}
