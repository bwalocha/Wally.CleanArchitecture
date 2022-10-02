using System;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

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
		var model = new User(name);
		model.AddDomainEvent(new UserCreatedDomainEvent(model.Id, model.Name));

		return model;
	}

	public static User Create(Guid id, string name)
	{
		var model = new User(id, name);
		model.AddDomainEvent(new UserCreatedDomainEvent(model.Id, model.Name));

		return model;
	}

	public void Update(string name)
	{
		Name = name;
	}
}
