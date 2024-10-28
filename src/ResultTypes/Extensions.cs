using ResultTypes.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes;

internal static class Extensions
{
    public static AggregateValidationException ToAggregateException(this IEnumerable<ValidationResult> results)
    {
        return new(results.Select(e => new ValidationException(e, null, null)));
    }
}
