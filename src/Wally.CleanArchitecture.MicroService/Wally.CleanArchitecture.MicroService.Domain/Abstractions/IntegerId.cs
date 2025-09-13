namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class IntegerId<TStronglyTypedId> : StronglyTypedId<TStronglyTypedId, uint>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, uint>
{
	protected IntegerId(uint value)
		: base(value)
	{
	}
}
