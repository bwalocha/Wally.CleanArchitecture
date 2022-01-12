using Wally.Lib.DDD.Abstractions.DomainModels;

namespace Wally.CleanArchitecture.Domain.Users;

public class User : AggregateRoot
{
	// Hide public .ctor
#pragma warning disable CS8618
	private User()
#pragma warning restore CS8618
	{
	}

	public string Name { get; private set; }

	public static User Create(string name)
	{
		return new User { Name = name, };
	}

	public void Update(string name)
	{
		Name = name;
	}
}
