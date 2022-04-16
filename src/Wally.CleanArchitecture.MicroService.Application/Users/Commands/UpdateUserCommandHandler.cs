using System.Threading;
using System.Threading.Tasks;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.MicroService.Application.Users.Commands;

public class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand>
{
	private readonly IUserRepository _userRepository;

	public UpdateUserCommandHandler(IUserRepository userRepository)
	{
		_userRepository = userRepository;
	}

	public override async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
	{
		var user = await _userRepository.GetAsync(command.Id, cancellationToken);

		user.Update(command.Name);

		_userRepository.Update(user);
	}
}
