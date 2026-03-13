namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

[ExcludeFromCodeCoverage]
public class PageInfoResult : IResult
{
	public PageInfoResult(int index, int size, int total)
	{
		Index = index;
		Size = size;
		Total = total;
	}

	public int Index { get; }

	public int Size { get; }

	public int Total { get; }
}
