using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts;

[ExcludeFromCodeCoverage]
public class PagedResponse<TResponse> : IResponse
{
	public PagedResponse(TResponse[] items, PageInfoResponse pageInfo)
	{
		Items = items;
		PageInfo = pageInfo;
	}

	public TResponse[] Items { get; }

	public PageInfoResponse PageInfo { get; }
}
