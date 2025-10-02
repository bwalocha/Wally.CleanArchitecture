using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult>
	where TQuery : IQuery<TResult>, IRequest<TResult>
	where TResult : IResult
{
	public abstract ValueTask<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);

	public ValueTask<TResult> Handle(TQuery request, CancellationToken cancellationToken)
	{
		return HandleAsync(request, cancellationToken);
	}
}
