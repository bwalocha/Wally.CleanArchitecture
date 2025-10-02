using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface ICommandAuthorizationHandler<in TCommand> : ICommandAuthorizationHandler<TCommand, Unit>
	where TCommand : ICommand<Unit>
{
}

public interface ICommandAuthorizationHandler<in TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	Task<AuthorizationHandlerResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public enum AuthorizationHandlerResult
{
	Unauthorized,
	Succeeded,
}
