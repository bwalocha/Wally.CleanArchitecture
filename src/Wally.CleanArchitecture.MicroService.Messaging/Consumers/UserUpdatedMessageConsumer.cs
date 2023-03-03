using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.IdentityProvider.Contracts.Messages;

namespace Wally.CleanArchitecture.MicroService.Messaging.Consumers;

public class UserUpdatedMessageConsumer : IConsumer<UserUpdatedMessage>
{
	private readonly IMediator _mediator;

	public UserUpdatedMessageConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<UserUpdatedMessage> context)
	{
		var message = context.Message;
		var command = new UpdateUserCommand(message.UserId, message.UserName);

		return _mediator.Send(command);
	}
}
