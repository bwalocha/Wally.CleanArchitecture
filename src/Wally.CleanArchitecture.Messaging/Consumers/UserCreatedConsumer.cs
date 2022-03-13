using MediatR;

using Microsoft.Extensions.Logging;

using Wally.CleanArchitecture.Application.Users.Commands;
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
		return new CreateUserCommand(message.UserId, message.UserName);
	}
}
