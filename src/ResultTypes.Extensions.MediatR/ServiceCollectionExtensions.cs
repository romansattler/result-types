using MediatR.Pipeline;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace ResultTypes.Extensions.MediatR;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddResultMediatRExceptionHandler(this IServiceCollection services)
    {
        services.TryAddEnumerable(ServiceDescriptor.Transient(typeof(IRequestExceptionHandler<,,>), typeof(ResultExceptionHandler<,,>)));

        return services;
    }
}
