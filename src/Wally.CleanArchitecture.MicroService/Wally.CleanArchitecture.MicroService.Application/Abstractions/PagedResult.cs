namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

[ExcludeFromCodeCoverage]
public class PagedResult<TResult> : IResult
	where TResult : IResult
{
	public PagedResult(TResult[] items)
		: this(items, new PageInfoResult(0, items.Length, items.Length))
	{
	}
	
	public PagedResult(TResult[] items, PageInfoResult pageInfo)
	{
		Items = items;
		PageInfo = pageInfo;
	}

	public TResult[] Items { get; }

	public PageInfoResult PageInfo { get; }
}
