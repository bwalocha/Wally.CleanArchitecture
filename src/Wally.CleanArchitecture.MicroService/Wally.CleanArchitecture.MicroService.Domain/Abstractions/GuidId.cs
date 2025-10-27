using System;

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
		// TODO: use .Net 9 Guid.CreateVersion7();
		return Ulid.NewUlid()
			.ToGuid();
	}
}
