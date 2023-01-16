using MediatR;

using Microsoft.Extensions.Logging;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.IdentityProvider.Contracts.Messages;
using Wally.Lib.DDD.Abstractions.Commands;
using Wally.Lib.ServiceBus.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Messaging.Consumers;

public class UserUpdatedMessageConsumer : Consumer<UserUpdatedMessage>
{
	public UserUpdatedMessageConsumer(IMediator mediator, ILogger<UserUpdatedMessageConsumer> logger)
		: base(mediator, logger)
	{
	}

	protected override ICommand CreateCommand(UserUpdatedMessage message)
	{
		return new UpdateUserCommand(message.UserId, message.UserName);
	}
}
