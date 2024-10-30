# ResultTypes
Result types for .NET Core including extensions for automatic handling in ASP.NET Core and [MediatR](https://github.com/jbogard/MediatR).

## Basic usage
Void value Result:
```csharp
internal class MyCommandHandler : IRequestHandler<MyCommand, Result>
{
	public async Task<Result> Handle(MyCommand request)
	{
		// do some work

		return Result.Success();
	}
}
```

Result with Value:
```csharp
internal class MyCommandHandler : IRequestHandler<MyCommand, Result<MyResponse>>
{
	public async Task<Result<MyResponse>> Handle(MyCommand request)
	{
		MyResponse response;

		// do some work

		return Result.Success(response);
	}
}
```

Invalid request:
```csharp
internal class MyCommandHandler : IRequestHandler<MyCommand, Result>
{
	public async Task<Result> Handle(MyCommand request)
	{
		if (!request.IsValid())
		{
			return Result.Invalid(new ValidationResult("The request is not valid"));
		}
		
		// do some work

		return Result.Success();
	}
}
```

Not found result:
```csharp
internal class MyCommandHandler(IMyRepository repo) : IRequestHandler<MyCommand, Result<Entity>>
{
	public async Task<Result<Entity>> Handle(MyCommand request)
	{
		var entity = await repo.Get(request.Id);

		return entity switch
		{
			null => Result.NotFound(),
			_ => Result.Success(entity)
		};
	}
}
```

Error result:
```csharp
internal class MyCommandHandler(IMyRepository repo) : IRequestHandler<MyCommand, Result<Entity>>
{
	public async Task<Result<Entity>> Handle(MyCommand request)
	{
		try
		{
			var entity = await repo.Get(request.Id);

			return Result.Success(entity)
		}
		catch (Exception ex)
		{
			return Result.Error(ex);
		}
	}
}
```

## Ensure success
Throw if Status is not Success
```csharp
internal class MyCommandHandler(IMediator mediator) : IRequestHandler<MyCommand, Result>
{
	public async Task<Result> Handle(MyCommand request)
	{
		var queryResult = await mediator.Send(new GetEntityByIdQuery(request.Id));

 		// throws appropriate Exception if queryResult.IsSuccess is false
		// the Exceptions can be automatically handled by ResultTypes.Extensions.MediatR
		queryResult.EnsureSuccess();

		// do some work

		return Result.Success();
	}
}
```

Throw if result has no value
```csharp
internal class MyCommandHandler(IMediator mediator) : IRequestHandler<MyCommand, Result>
{
	public async Task<Result> Handle(MyCommand request)
	{
		var queryResult = await mediator.Send(new GetEntityByIdQuery(request.Id));

		Entity entity;
		entity = queryResult.Value; // might be null

 		// throws appropriate Exception if queryResult.HasValue is false
		// the Exceptions can be automatically handled by ResultTypes.Extensions.MediatR
		queryResult.EnsureValue();

		entity = queryResult.Value; // will be not null

		// do some work

		return Result.Success();
	}
}
```


## Automatic conversion
Implicit success result:
```csharp
internal class MyCommandHandler : IRequestHandler<MyCommand, Result<MyResponse>>
{
	public async Task<Result<MyResponse>> Handle(MyCommand request)
	{
		MyResponse response;

		// do some work

		return response;
	}
}
```

## ASP.NET Core integration
Use instead of [`Microsoft.AspNetCore.Http.TypedResult`](https://learn.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.http.typedresults):
```csharp
public static class Endpoints
{
    public static void MapGetEntity(this IEndpointRouteBuilder app)
    {
        app.MapGet("/{id:guid}", GetEntity);
    }

    public static async Task<ResultType.Ok<Entity>> GetEntity([FromServices] IMediator mediator, [FromRoute] Guid id)
    {
        return await mediator.Send(new GetEntityQuery(id));
    }
}
```

Returning any `ResultType` will automatically the API Explorer and therefore swagger with the correct return type and status codes.

The `Result` will be implicitly converted to `ResultTypes.Extensions.AspNetCore.ResultType`.

## MediatR integration
Register request exception handler:
```csharp
using ResultTypes.Extensions.MediatR;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddServices(this IServiceCollection services)
	{
		services.AddResultMediatRExceptionHandler();

		return services;
	}
}
```

If the response is a `Result`, the exception handler will map exceptions of type `ValidationException` to `Result.Invalid` and all other exceptions to `Result.Error`.
Exceptions thrown by `Result.EnsureSuccess` and `Result.EnsureValue` will be mapped back to the original `Result` type.