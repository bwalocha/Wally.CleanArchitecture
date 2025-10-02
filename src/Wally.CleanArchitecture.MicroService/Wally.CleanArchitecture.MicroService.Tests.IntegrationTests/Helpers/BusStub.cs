using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Helpers;

public class BusStub : IBus
{
	public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken)
		where TMessage : class
	{
		return Task.CompletedTask;
	}
}
