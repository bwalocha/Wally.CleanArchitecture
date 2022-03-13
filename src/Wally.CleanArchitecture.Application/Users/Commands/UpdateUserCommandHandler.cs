using System.Threading;
using System.Threading.Tasks;

using Wally.CleanArchitecture.Application.Abstractions;

namespace Wally.CleanArchitecture.Application.Users.Commands;

public class UpdateUserCommandHandler : CommandHandler<UpdateUserCommand>
{
	private readonly IUserRepository _usersRepository;

	public UpdateUserCommandHandler(IUserRepository usersRepository)
	{
		_usersRepository = usersRepository;
	}

	public override async Task HandleAsync(UpdateUserCommand command, CancellationToken cancellationToken)
	{
		var user = await _usersRepository.GetAsync(command.Id, cancellationToken);

		user.Update(command.Name);

		_usersRepository.Update(user);
	}
}
