namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IEventHub
{
	public Task NewEventAsync(string user, string @event, CancellationToken cancellationToken);
}
