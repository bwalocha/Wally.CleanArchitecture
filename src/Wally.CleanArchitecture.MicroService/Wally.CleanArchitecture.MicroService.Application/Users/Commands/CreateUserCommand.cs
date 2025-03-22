using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

[ExcludeFromCodeCoverage]
public sealed class CreateUserCommand : ICommand<UserId>
{
	public CreateUserCommand(UserId userId, string name)
	{
		UserId = userId;
		Name = name;
	}

	public CreateUserCommand(string name)
		: this(new UserId(), name)
	{
	}

	public UserId UserId { get; }

	public string Name { get; }
}
