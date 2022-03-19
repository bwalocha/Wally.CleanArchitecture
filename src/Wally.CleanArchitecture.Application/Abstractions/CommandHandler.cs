using System.Threading;
using System.Threading.Tasks;

using MediatR;

using Wally.Lib.DDD.Abstractions.Commands;

namespace Wally.CleanArchitecture.Application.Abstractions;

public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand, IRequest
{
	public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);

	public async Task<Unit> Handle(TCommand command, CancellationToken cancellationToken)
	{
		await HandleAsync(command, cancellationToken);

		return Unit.Value;
	}
}
