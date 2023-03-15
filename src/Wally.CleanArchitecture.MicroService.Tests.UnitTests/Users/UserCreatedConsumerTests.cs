using System;
using System.Threading;
using System.Threading.Tasks;

using MassTransit;

using MediatR;

using Moq;

using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;
using Wally.Identity.Messages.Users;

using Xunit;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Users;

public class UserCreatedConsumerTests
{
	private readonly UserCreatedMessageConsumer _consumer;
	private readonly Mock<IMediator> _mediatorMock;

	public UserCreatedConsumerTests()
	{
		_mediatorMock = new Mock<IMediator>();
		_consumer = new UserCreatedMessageConsumer(_mediatorMock.Object);
	}

	[Fact]
	public async Task ConsumeAsync_ForValidMessage_ShouldPublishCommand()
	{
		// Arrange
		var message = new UserCreatedMessage(Guid.NewGuid(), "testName", "test@email.com");
		var context = new Mock<ConsumeContext<UserCreatedMessage>>();
		context.SetupGet(a => a.Message)
			.Returns(message);

		// Act
		await _consumer.Consume(context.Object);

		// Assert
		_mediatorMock.Verify(
			a => a.Send(
				It.Is<CreateUserCommand>(a => a.Id == message.UserId && a.Name == message.UserName),
				CancellationToken.None),
			Times.Once());
	}
}
