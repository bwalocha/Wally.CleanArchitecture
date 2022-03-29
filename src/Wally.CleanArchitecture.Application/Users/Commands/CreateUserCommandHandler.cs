using System.Threading;
using System.Threading.Tasks;

using Wally.CleanArchitecture.Domain.Users;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.Application.Users.Commands;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand>
{
	private readonly IUserRepository _usersRepository;

	public CreateUserCommandHandler(IUserRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	public override Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
	{
		var user = User.Create(command.Id, command.Name);

		_usersRepository.Add(user);

		return Task.CompletedTask;
	}
}
