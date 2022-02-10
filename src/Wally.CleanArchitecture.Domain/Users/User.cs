using System;
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

	private User(Guid id, string name)
		: base(id)
	{
		Name = name;
	}
	
	private User(string name)
	{
		Name = name;
	}

	public string Name { get; private set; }

	public static User Create(string name)
	{
		return new User(name);
	}

	public static User Create(Guid id, string name)
	{
		return new User(id, name);
	}

	public void Update(string name)
	{
		Name = name;
	}
}
