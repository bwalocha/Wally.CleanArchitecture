using System.Threading;
using System.Threading.Tasks;

using Wally.Lib.ServiceBus.Abstractions;

namespace Wally.CleanArchitecture.MicroService.IntegrationTests;

public class PublisherStub : IPublisher
{
	public Task PublishAsync<TMessage>(TMessage message, CancellationToken cancellationToken) where TMessage : class
	{
		return Task.CompletedTask;
	}
}
