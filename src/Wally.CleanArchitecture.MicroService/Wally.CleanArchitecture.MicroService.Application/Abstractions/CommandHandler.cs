using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public abstract class CommandHandler<TCommand>
	: ICommandHandler<TCommand>
	where TCommand : ICommand
{
	public abstract Task HandleAsync(TCommand command, CancellationToken cancellationToken);

	public async Task<Unit> Handle(TCommand request, CancellationToken cancellationToken)
	{
		await HandleAsync(request, cancellationToken);
		return Unit.Value;
	}
}

public abstract class CommandHandler<TCommand, TResult>
	: ICommandHandler<TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	public abstract Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);

	public Task<TResult> Handle(TCommand request, CancellationToken cancellationToken)
	{
		return HandleAsync(request, cancellationToken);
	}
}
