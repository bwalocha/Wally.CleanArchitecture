using System;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Domain.Users;

public class User : AggregateRoot<User, UserId>, ISoftDeletable/*, ITemporal*/
{
	public const int NameMaxLength = 256;
	
	// Hide public .ctor
	private User()
	{
	}

	private User(UserId id)
		: base(id)
	{
	}

	public string Name { get; private set; } = null!;

	public bool IsDeleted { get; private set; } = false;
	public DateTimeOffset? DeletedAt { get; private set; } = null;

	public UserId? DeletedById { get; private set; } = null;

	public static User Create(string name)
	{
		var model = new User
		{
			Name = name, 
		};
		model.AddDomainEvent(new UserCreatedDomainEvent(model.Id));

		return model;
	}

	public static User Create(UserId id, string name)
	{
		var model = new User(id)
		{
			Name = name, 
		};
		model.AddDomainEvent(new UserCreatedDomainEvent(model.Id));

		return model;
	}

	public User Update(string name)
	{
		Name = name;

		return this;
	}

	/*public DateTime ValidFrom { get; private set; } = DateTime.MinValue;
	public DateTime ValidTo { get; private set; } = DateTime.MaxValue;
	public bool IsActiveAt(DateTimeOffset date) => date >= ValidFrom && date < ValidTo;*/
}
