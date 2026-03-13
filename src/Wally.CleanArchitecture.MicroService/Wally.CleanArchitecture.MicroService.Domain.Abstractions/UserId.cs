namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public sealed class UserId : GuidId<UserId>
{
	public UserId()
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
