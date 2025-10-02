using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public abstract class CommandHandler<TCommand>
	: ICommandHandler<TCommand>
	where TCommand : ICommand
{
	public abstract ValueTask HandleAsync(TCommand command, CancellationToken cancellationToken);

	public async ValueTask<Unit> Handle(TCommand request, CancellationToken cancellationToken)
	{
		await HandleAsync(request, cancellationToken);
		return Unit.Value;
	}
}

public abstract class CommandHandler<TCommand, TResult>
	: ICommandHandler<TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	public abstract ValueTask<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);

	public ValueTask<TResult> Handle(TCommand request, CancellationToken cancellationToken)
	{
		return HandleAsync(request, cancellationToken);
	}
}
