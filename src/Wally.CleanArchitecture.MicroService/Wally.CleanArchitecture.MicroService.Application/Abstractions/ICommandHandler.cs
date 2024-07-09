using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Unit>
	where TCommand : ICommand
{
	Task HandleAsync(TCommand command, CancellationToken cancellationToken);
}

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
	where TCommand : ICommand<TResult>
{
	Task<TResult> HandleAsync(TCommand command, CancellationToken cancellationToken);
}
