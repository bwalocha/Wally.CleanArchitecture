using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Contracts;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

[ExcludeFromCodeCoverage]
public class PagedQuery<TRequest, TResponse> : IQuery<PagedResponse<TResponse>>
	where TRequest : IRequest
	where TResponse : IResponse
{
	protected PagedQuery(ODataQueryOptions<TRequest> queryOptions)
	{
		QueryOptions = queryOptions;
	}

	public ODataQueryOptions<TRequest> QueryOptions { get; }
}
