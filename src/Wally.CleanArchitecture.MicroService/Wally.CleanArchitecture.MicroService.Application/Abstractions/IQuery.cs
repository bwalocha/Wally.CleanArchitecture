using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IQuery<out TResult> : IRequest<TResult>
	where TResult : IResult
{
}
