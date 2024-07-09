using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult>
	where TQuery : IQuery<TResult>
	where TResult : IResponse
{
	Task<TResult> HandleAsync(TQuery query, CancellationToken cancellationToken);
}
