using Microsoft.AspNetCore.Http.HttpResults;
using HttpResults = Microsoft.AspNetCore.Http.HttpResults;

namespace ResultTypes.Extensions.AspNetCore;

public static class ResultType
{
    public sealed class Ok<TValue>(Result<TValue> result) : ResultType<HttpResults.Ok<TValue>, TValue>(result)
    {
        public static implicit operator Ok<TValue>(Result<TValue> result) => new(result);
    }

    public sealed class Collection<TValue>(Result<IEnumerable<TValue>> result) : ResultType<Results<HttpResults.Ok<IEnumerable<TValue>>, HttpResults.NoContent>, IEnumerable<TValue>>(result)
    {
        public static implicit operator Collection<TValue>(Result<IEnumerable<TValue>> result) => new(result);
    }

    public sealed class Created<TValue>(Result<TValue> result) : ResultType<HttpResults.Created<TValue>, TValue>(result)
    {
        public static implicit operator Created<TValue>(Result<TValue> result) => new(result);
    }

    public sealed class Created(Result<Unit> result) : ResultType<HttpResults.Created, Unit>(result)
    {
        public static implicit operator Created(Result<Unit> result) => new(result);
    }

    public sealed class NoContent(Result<Unit> result) : ResultType<HttpResults.NoContent, Unit>(result)
    {
        public static implicit operator NoContent(Result<Unit> result) => new(result);
    }
}