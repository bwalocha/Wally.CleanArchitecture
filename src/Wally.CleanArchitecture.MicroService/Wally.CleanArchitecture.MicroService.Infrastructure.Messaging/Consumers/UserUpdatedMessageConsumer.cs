using System.Threading.Tasks;
using MassTransit;
using Mediator;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Domain.Users;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;

// TODO: create abstract base class and set RequestContext properties
public class UserUpdatedMessageConsumer : IConsumer<UserUpdatedMessage>
{
	private readonly ISender _mediator;

	public UserUpdatedMessageConsumer(ISender mediator)
	{
		_mediator = mediator;
	}

	public async Task Consume(ConsumeContext<UserUpdatedMessage> context)
	{
		var message = context.Message;
		var command = new UpdateUserCommand(new UserId(message.UserId), message.UserName);

		await _mediator.Send(command);
	}
}
