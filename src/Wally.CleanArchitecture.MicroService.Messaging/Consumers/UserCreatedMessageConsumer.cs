using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Messaging.Consumers;

public class UserCreatedMessageConsumer : IConsumer<UserCreatedMessage>
{
	private readonly IMediator _mediator;

	public UserCreatedMessageConsumer(IMediator mediator)
	{
		_mediator = mediator;
	}

	public Task Consume(ConsumeContext<UserCreatedMessage> context)
	{
		var message = context.Message;
		var command = new CreateUserCommand(message.UserId, message.UserName);

		return _mediator.Send(command);
	}
}
