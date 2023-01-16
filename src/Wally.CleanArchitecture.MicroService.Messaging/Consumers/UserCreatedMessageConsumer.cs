using MediatR;

using Microsoft.Extensions.Logging;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.IdentityProvider.Contracts.Messages;
using Wally.Lib.DDD.Abstractions.Commands;
using Wally.Lib.ServiceBus.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Messaging.Consumers;

public class UserCreatedMessageConsumer : Consumer<UserCreatedMessage>
{
	public UserCreatedMessageConsumer(IMediator mediator, ILogger<UserCreatedMessageConsumer> logger)
		: base(mediator, logger)
	{
	}

	protected override ICommand CreateCommand(UserCreatedMessage message)
	{
		return new CreateUserCommand(message.UserId, message.UserName);
	}
}
