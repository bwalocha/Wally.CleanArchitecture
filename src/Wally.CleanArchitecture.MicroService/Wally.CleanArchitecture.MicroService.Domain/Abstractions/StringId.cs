using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class StringId<TStronglyTypedId> : StronglyTypedId<TStronglyTypedId, string>
	where TStronglyTypedId : StronglyTypedId<TStronglyTypedId, string>
{
	protected StringId()
		: this(NewSequentialId())
	{
	}

	protected StringId(string value)
		: base(value)
	{
	}

	private static string NewSequentialId()
	{
		return DateTime.UnixEpoch.Ticks.ToString();
	}
}
