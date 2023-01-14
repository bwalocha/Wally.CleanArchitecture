using System;

namespace Wally.CleanArchitecture.MicroService.Messages.Users;

public class UserCreatedMessage
{
	public UserCreatedMessage(Guid id, string name)
	{
		Id = id;
		Name = name;

		new UserCreatedMessageValidator().Validate(this);
	}

	public Guid Id { get; }

	public string Name { get; }
}
