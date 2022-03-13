using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.OData.Query;

using Wally.Lib.DDD.Abstractions.DomainModels;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Domain.Abstractions;

public interface IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
{
	Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

	Task<TAggregateRoot> GetAsync(Guid id, CancellationToken cancellationToken);

	Task<TResult> GetAsync<TResult>(Guid id, CancellationToken cancellationToken) where TResult : IResponse;

	Task<PagedResponse<TResponse>> GetAsync
		<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest where TResponse : class, IResponse;

	TAggregateRoot Add(TAggregateRoot aggregateRoot);

	TAggregateRoot Update(TAggregateRoot aggregateRoot);

	TAggregateRoot Remove(TAggregateRoot aggregateRoot);

	TEntity Attach<TEntity>(TEntity entity) where TEntity : Entity;
}
