using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

public class UserId : StronglyTypedId<UserId, Guid>
{
	public UserId()
		: this(Guid.NewGuid())
	{
	}

	public UserId(Guid value)
		: base(value)
	{
	}

	public static explicit operator Guid(UserId id)
	{
		return id.Value;
	}
}
