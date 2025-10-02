using Microsoft.AspNetCore.OData.Query;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

[ExcludeFromCodeCoverage]
public class PagedQuery<TRequest, TResult> : IQuery<PagedResult<TResult>>
	where TRequest : IRequest
	where TResult : IResult
{
	protected PagedQuery(ODataQueryOptions<TRequest> queryOptions)
	{
		QueryOptions = queryOptions;
	}

	public ODataQueryOptions<TRequest> QueryOptions { get; }
}
