using Wally.CleanArchitecture.MicroService.Application.Contracts.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Contracts;

[ExcludeFromCodeCoverage]
public class PageInfoResponse : IResponse
{
	public PageInfoResponse(int index, int size, int total)
	{
		Index = index;
		Size = size;
		Total = total;
	}

	public int Index { get; }

	public int Size { get; }

	public int Total { get; }
}
