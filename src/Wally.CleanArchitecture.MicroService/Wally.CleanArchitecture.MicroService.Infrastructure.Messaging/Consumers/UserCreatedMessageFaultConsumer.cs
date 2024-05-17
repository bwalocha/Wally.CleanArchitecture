using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;

public class UserCreatedMessageFaultConsumer : IConsumer<Fault<UserCreatedMessage>>
{
	private readonly ILogger<UserCreatedMessageFaultConsumer> _logger;
	
	public UserCreatedMessageFaultConsumer(ILogger<UserCreatedMessageFaultConsumer> logger)
	{
		_logger = logger;
	}
	
	public Task Consume(ConsumeContext<Fault<UserCreatedMessage>> context)
	{
		_logger.LogCritical("Massage '{ContextMessage}' processing error: {ContextMessageId}", context.Message,
			context.MessageId);
		
		return Task.CompletedTask;
	}
}
