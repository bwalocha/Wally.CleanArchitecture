namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class GuidId<TStronglyTypedId> : StronglyTypedId<TStronglyTypedId, Guid>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, Guid>
{
	protected GuidId()
		: this(NewSequentialId())
	{
	}

	protected GuidId(Guid value)
		: base(value)
	{
	}

	private static Guid NewSequentialId()
	{
		return Guid.CreateVersion7();
	}
}
