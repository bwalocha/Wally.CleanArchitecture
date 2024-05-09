using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class LongId<TStronglyTypedId> : StronglyTypedId<TStronglyTypedId, long>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, long>
{
	protected LongId()
		: this(NewSequentialId())
	{
	}

	protected LongId(long value)
		: base(value)
	{
	}

	private static long NewSequentialId()
	{
		return DateTime.UnixEpoch.Ticks;
	}
}
