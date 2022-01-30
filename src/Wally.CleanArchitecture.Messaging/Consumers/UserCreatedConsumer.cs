using MediatR;
using Microsoft.Extensions.Logging;
using Wally.IdentityProvider.Contracts.Messages;
using Wally.Lib.DDD.Abstractions.Commands;
using Wally.Lib.ServiceBus.Abstractions;

namespace Wally.CleanArchitecture.Messaging.Consumers;

public class UserCreatedConsumer : Consumer<UserCreatedMessage>
{
	public UserCreatedConsumer(IMediator mediator, ILogger<UserCreatedConsumer> logger)
		: base(mediator, logger)
	{
	}

	protected override ICommand CreateCommand(UserCreatedMessage message)
	{
		throw new NotImplementedException();
	}
}
