using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OData.Query;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IReadOnlyRepository<TEntity, in TKey>
	where TEntity : Entity<TEntity, TKey> where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken);

	Task<TResponse> GetAsync<TResponse>(TKey id, CancellationToken cancellationToken)
		where TResponse : IResponse;

	Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest where TResponse : class, IResponse;
}
