namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IBus
{
	Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
		where TMessage : class;
}
