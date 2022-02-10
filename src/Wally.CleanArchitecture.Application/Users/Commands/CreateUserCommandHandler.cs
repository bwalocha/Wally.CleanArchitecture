using System.Threading;
using System.Threading.Tasks;
using Wally.CleanArchitecture.Application.Abstractions;
using Wally.CleanArchitecture.Domain.Users;

namespace Wally.CleanArchitecture.Application.Users.Commands;

public class CreateUserCommandHandler : CommandHandler<UpdateUserCommand>
{
	private readonly IUserRepository _usersRepository;

	public CreateUserCommandHandler(IUserRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	public override Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
	{
		var user = User.Create(command.Id, command.Name);

		_usersRepository.Add(user);
		
		return Task.CompletedTask;
	}
}
