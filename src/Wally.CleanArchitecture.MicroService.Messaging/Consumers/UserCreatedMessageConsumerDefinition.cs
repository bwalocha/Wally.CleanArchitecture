using System;

using MassTransit;

namespace Wally.CleanArchitecture.MicroService.Messaging.Consumers;

public class UserCreatedMessageConsumerDefinition : ConsumerDefinition<UserCreatedMessageConsumer>
{
	public UserCreatedMessageConsumerDefinition()
	{
		// limit the number of messages consumed concurrently
		// this applies to the consumer only, not the endpoint
		ConcurrentMessageLimit = 4;
	}

	protected override void ConfigureConsumer(
		IReceiveEndpointConfigurator endpointConfigurator,
		IConsumerConfigurator<UserCreatedMessageConsumer> consumerConfigurator)
	{
		endpointConfigurator.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(60)));
		endpointConfigurator.UseInMemoryOutbox();
	}
}
