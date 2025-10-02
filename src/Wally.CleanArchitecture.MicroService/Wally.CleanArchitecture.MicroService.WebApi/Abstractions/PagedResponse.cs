namespace Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

[ExcludeFromCodeCoverage]
public class PagedResponse<TResponse> : IResponse
	where TResponse : IResponse
{
	public PagedResponse(TResponse[] items)
		: this(items, new PageInfoResponse(0, items.Length, items.Length))
	{
	}
	
	public PagedResponse(TResponse[] items, PageInfoResponse pageInfo)
	{
		Items = items;
		PageInfo = pageInfo;
	}

	public TResponse[] Items { get; }

	public PageInfoResponse PageInfo { get; }
}
