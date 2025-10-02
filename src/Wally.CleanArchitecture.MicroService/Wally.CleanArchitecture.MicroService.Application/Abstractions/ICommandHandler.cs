using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
	where TCommand : ICommand
{
	ValueTask HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	ValueTask<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
