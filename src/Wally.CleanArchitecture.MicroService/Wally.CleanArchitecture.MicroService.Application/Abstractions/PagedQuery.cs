namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

[ExcludeFromCodeCoverage]
public class PagedQuery<TRequest, TResult> : IQuery<PagedResult<TResult>>
	where TRequest : IRequest
	where TResult : IResult
{
	protected PagedQuery(IQueryOptions<TRequest> queryOptions)
	{
		QueryOptions = queryOptions;
	}

	public IQueryOptions<TRequest> QueryOptions { get; }
}
