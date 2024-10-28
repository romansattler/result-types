using Microsoft.AspNetCore.Http.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ResultTypes.Extensions.AspNetCore;

public class ResultType<TSuccessResult, TValue>(Result<TValue> result) : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
    where TSuccessResult : IResult, IEndpointMetadataProvider
{
    public TValue? Value => typeof(TValue) != typeof(Unit) ? result.Value : default;
    object? IValueHttpResult.Value => Value;

    public int? StatusCode { get; }

    static void IEndpointMetadataProvider.PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(method);
        ArgumentNullException.ThrowIfNull(builder);

        TSuccessResult.PopulateMetadata(method, builder);

        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status400BadRequest, typeof(void)));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status404NotFound, typeof(void)));
        builder.Metadata.Add(new ProducesResponseTypeMetadata(StatusCodes.Status500InternalServerError, typeof(void)));
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        await (result switch
        {
            { Status: ResultStatus.Ok, Value: IEnumerable<object>[] } => TypedResults.NoContent().ExecuteAsync(httpContext),
            { Status: ResultStatus.Ok, Value: { } value } => TypedResults.Ok(value).ExecuteAsync(httpContext),
            { Status: ResultStatus.Ok } => TypedResults.NoContent().ExecuteAsync(httpContext),
            { Status: ResultStatus.Created, Id: { } id } => TypedResults.Created(new Uri($"{id}", UriKind.Relative), Value).ExecuteAsync(httpContext),
            { Status: ResultStatus.Created } => TypedResults.Created((string?)null, Value).ExecuteAsync(httpContext),
            { Status: ResultStatus.Invalid, ValidationErrors: { Length: > 0 } validationErrors } => TypedResults.ValidationProblem(MapValidationResults(validationErrors)).ExecuteAsync(httpContext),
            { Status: ResultStatus.Invalid } => TypedResults.BadRequest().ExecuteAsync(httpContext),
            { Status: ResultStatus.NotFound } => TypedResults.NotFound().ExecuteAsync(httpContext),
            _ => TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError).ExecuteAsync(httpContext)
        });
    }

    private static Dictionary<string, string[]> MapValidationResults(IReadOnlyCollection<ValidationResult> validationResults)
    {
        return validationResults
            .SelectMany(r => r.MemberNames, (r, m) => new { MemberName = m, Message = r.ErrorMessage })
            .Where(v => !string.IsNullOrEmpty(v.MemberName))
            .Select(v => new { v.MemberName, Message = v.Message! })
            .ToLookup(r => r.MemberName, r => r.Message)
            .ToDictionary(g => g.Key, g => g.ToArray());
    }
}
