using System.Threading.Tasks;
using MassTransit;
using Mediator;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;

// TODO: create abstract base class and set RequestContext properties
public class UserCreatedMessageConsumer : IConsumer<UserCreatedMessage>
{
	private readonly ISender _mediator;

	public UserCreatedMessageConsumer(ISender mediator)
	{
		_mediator = mediator;
	}

	public async Task Consume(ConsumeContext<UserCreatedMessage> context)
	{
		var message = context.Message;
		var command = new CreateUserCommand(new UserId(message.UserId), message.UserName);

		await _mediator.Send(command);
	}
}
