using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Wally.Lib.DDD.Abstractions.Queries;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Application.Abstractions;

public abstract class QueryHandler<TQuery, TResponse> : IQueryHandler<TQuery, TResponse>
	where TQuery : IQuery<TResponse>, IRequest<TResponse> where TResponse : IResponse
{
	public abstract Task<TResponse> HandleAsync(TQuery query, CancellationToken cancellationToken);

	public Task<TResponse> Handle(TQuery query, CancellationToken cancellationToken)
	{
		return HandleAsync(query, cancellationToken);
	}
}
