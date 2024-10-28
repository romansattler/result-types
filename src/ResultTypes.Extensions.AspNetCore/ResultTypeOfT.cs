using Microsoft.AspNetCore.Http.Metadata;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace ResultTypes.Extensions.AspNetCore;

public class ResultType<TSuccessResult, TValue>(Result<TValue> result) : IResult, IEndpointMetadataProvider, IStatusCodeHttpResult, IValueHttpResult, IValueHttpResult<TValue>
    where TSuccessResult : IResult, IEndpointMetadataProvider
{
    public TValue? Value => typeof(TValue) != typeof(Unit) ? result.Value : default;
    object? IValueHttpResult.Value => Value;

    public int? StatusCode => (MapResult(result) as IStatusCodeHttpResult)?.StatusCode;

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
        await MapResult(result).ExecuteAsync(httpContext);
    }

    private static IResult MapResult(Result<TValue> result)
    {
        return result switch
        {
            { Status: ResultStatus.Ok, Value: IEnumerable<object>[] } => TypedResults.NoContent(),
            { Status: ResultStatus.Ok, Value: Unit } => TypedResults.NoContent(),
            { Status: ResultStatus.Ok, Value: { } value } => TypedResults.Ok(value),
            { Status: ResultStatus.Ok } => TypedResults.NoContent(),
            { Status: ResultStatus.Created, Id: { } id, Value: var value } => TypedResults.Created(new Uri($"{id}", UriKind.Relative), value),
            { Status: ResultStatus.Created, Value: var value } => TypedResults.Created((string?)null, value),
            { Status: ResultStatus.Invalid, ValidationErrors: { Length: > 0 } validationErrors } => TypedResults.ValidationProblem(MapValidationResults(validationErrors)),
            { Status: ResultStatus.Invalid } => TypedResults.BadRequest(),
            { Status: ResultStatus.NotFound } => TypedResults.NotFound(),
            _ => TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError),
        };
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
