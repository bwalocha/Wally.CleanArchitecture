using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
	where TQuery : IQuery<TResult>
	where TResult : IResult
{
	ValueTask<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
