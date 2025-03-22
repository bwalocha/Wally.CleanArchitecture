using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

[ExcludeFromCodeCoverage]
public sealed class UpdateUserCommand : ICommand
{
	public UpdateUserCommand(UserId userId, string name)
	{
		UserId = userId;
		Name = name;
	}

	public UserId UserId { get; }

	public string Name { get; }
}
