using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.AspNetCore.OData.Query;

using Wally.Lib.DDD.Abstractions.DomainModels;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IReadOnlyRepository<TEntity> where TEntity : Entity
{
	Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);

	Task<TResponse> GetAsync<TResponse>(Guid id, CancellationToken cancellationToken) where TResponse : IResponse;

	Task<PagedResponse<TResponse>> GetAsync
		<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest where TResponse : class, IResponse;
}
