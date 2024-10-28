using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ResultTypes.Exceptions;

public sealed class AggregateValidationException(IEnumerable<ValidationException> validationExceptions) : AggregateException(validationExceptions)
{
    public new ReadOnlyCollection<ValidationException> InnerExceptions => base.InnerExceptions.Cast<ValidationException>().ToList().AsReadOnly();
}
