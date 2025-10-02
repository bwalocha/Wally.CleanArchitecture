using Mediator;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface ICommand : ICommand<Unit>
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}
