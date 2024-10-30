using Microsoft.AspNetCore.Http.Metadata;

namespace ResultTypes.Extensions.AspNetCore;

public class ResultType<TSuccessResult>(Result<Unit> result) : ResultType<TSuccessResult, Unit>(result)
    where TSuccessResult : IResult, IEndpointMetadataProvider
{
}
