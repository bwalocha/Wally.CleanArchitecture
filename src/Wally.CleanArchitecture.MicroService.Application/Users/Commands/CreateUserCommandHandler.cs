using System.Threading;
using System.Threading.Tasks;

using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand>
{
	private readonly IUserRepository _userRepository;

	public CreateUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override Task HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
	{
		var user = User.Create(command.Id, command.Name);

		_userRepository.Add(user);

		return Task.CompletedTask;
	}
}
