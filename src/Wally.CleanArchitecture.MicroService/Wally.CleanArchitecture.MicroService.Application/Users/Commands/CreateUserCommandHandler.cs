using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class CreateUserCommandHandler : CommandHandler<CreateUserCommand, UserId>
{
	private readonly IUserRepository _userRepository;

	public CreateUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override ValueTask<UserId> HandleAsync(CreateUserCommand command, CancellationToken cancellationToken)
	{
		var model = User.Create(command.UserId, command.Name);

		_userRepository.Add(model);

		return ValueTask.FromResult(model.Id);
	}
}
