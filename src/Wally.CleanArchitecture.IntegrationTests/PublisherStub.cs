using System.Threading;
using System.Threading.Tasks;

using Wally.Lib.DDD.Abstractions.DomainEvents;
using Wally.Lib.ServiceBus.Abstractions;

namespace Wally.CleanArchitecture.IntegrationTests;

public class PublisherStub : IPublisher
{
	public Task PublishAsync<TDomainEvent>(TDomainEvent domainEvent, CancellationToken cancellationToken)
		where TDomainEvent : DomainEvent
	{
		return Task.CompletedTask;
	}
}
