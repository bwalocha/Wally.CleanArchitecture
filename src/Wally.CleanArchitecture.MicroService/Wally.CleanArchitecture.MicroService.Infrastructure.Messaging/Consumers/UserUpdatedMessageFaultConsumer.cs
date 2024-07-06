using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;

public class UserUpdatedMessageFaultConsumer : IConsumer<Fault<UserUpdatedMessage>>
{
	private readonly ILogger<UserUpdatedMessageFaultConsumer> _logger;

	public UserUpdatedMessageFaultConsumer(ILogger<UserUpdatedMessageFaultConsumer> logger)
	{
		_logger = logger;
	}

	public Task Consume(ConsumeContext<Fault<UserUpdatedMessage>> context)
	{
		_logger.LogCritical("Massage '{ContextMessage}' processing error: {ContextMessageId}", context.Message,
			context.MessageId);

		return Task.CompletedTask;
	}
}
