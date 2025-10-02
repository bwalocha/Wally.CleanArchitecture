using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand>
{
	private readonly IUserRepository _userRepository;

	public UpdateUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override async ValueTask HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
	{
		var model = await _userRepository.GetAsync(command.UserId, cancellationToken);

		model.Update(command.Name);

		_userRepository.Update(model);
	}
}
