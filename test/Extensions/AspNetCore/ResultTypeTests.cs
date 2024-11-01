﻿using ResultTypes.Extensions.AspNetCore;

namespace ResultTypes.Tests.Extensions.AspNetCore;

public class ResultTypeTests
{
    [Fact]
    public void Implicit_FromSuccessWithValueResult_ReturnsOkResultType()
    {
        // Arrange
        var value = new TestValue();
        var result = Result.Success(value);

        // Act
        var resultType = (ResultType.Ok<TestValue>)result;

        // Assert
        Assert.IsType<ResultType.Ok<TestValue>>(resultType);
    }

    [Fact]
    public void Implicit_FromSuccessWithoutValueResult_ReturnsNoContentType()
    {
        // Arrange
        var result = Result.Success();

        // Act
        var resultType = (ResultType.NoContent)result;

        // Assert
        Assert.IsType<ResultType.NoContent>(resultType);
    }

    [Fact]
    public void Implicit_FromSuccessOfEnumerableResult_ReturnsCollectionType()
    {
        // Arrange
        IEnumerable<TestValue> values = [new(), new()];
        var result = Result.Success(values);

        // Act
        var resultType = (ResultType.Collection<TestValue>)result;

        // Assert
        Assert.IsType<ResultType.Collection<TestValue>>(resultType);
    }

    [Fact]
    public void Implicit_FromCreatedResult_ReturnsCreatedResultType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var value = new TestValue();
        var result = Result.Created(id, value);

        // Act
        var resultType = (ResultType.Created<TestValue>)result;

        // Assert
        Assert.IsType<ResultType.Created<TestValue>>(resultType);
    }

    [Fact]
    public void Implicit_FromCreatedResultWithoutValue_ReturnsCreatedType()
    {
        // Arrange
        var id = Guid.NewGuid();
        var result = Result.Created(id);

        // Act
        var resultType = (ResultType.Created)result;

        // Assert
        Assert.IsType<ResultType.Created>(resultType);
    }
}
