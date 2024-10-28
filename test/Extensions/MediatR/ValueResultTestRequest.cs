using MediatR;

namespace ResultTypes.Tests.Extensions.MediatR;

internal class ValueResultTestRequest : IRequest<Result<TestValue>>;