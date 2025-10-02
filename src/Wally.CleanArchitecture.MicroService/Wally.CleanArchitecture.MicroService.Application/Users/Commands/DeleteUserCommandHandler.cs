using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class DeleteUserCommandHandler : CommandHandler<DeleteUserCommand>
{
	private readonly IUserRepository _userRepository;

	public DeleteUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override async ValueTask HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
	{
		var model = await _userRepository.GetAsync(command.UserId, cancellationToken);

		_userRepository.Remove(model);
	}
}
