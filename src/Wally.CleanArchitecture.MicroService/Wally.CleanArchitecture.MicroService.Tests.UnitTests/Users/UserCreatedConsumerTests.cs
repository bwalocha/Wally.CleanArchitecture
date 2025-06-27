using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MediatR;
using Moq;
using Wally.CleanArchitecture.MicroService.Application.Users.Commands;
using Wally.CleanArchitecture.MicroService.Infrastructure.Messaging.Consumers;
using Wally.Identity.Messages.Users;

namespace Wally.CleanArchitecture.MicroService.Tests.UnitTests.Users;

public class UserCreatedConsumerTests
{
	private readonly UserCreatedMessageConsumer _sut;
	private readonly Mock<ISender> _mediatorMock;

	public UserCreatedConsumerTests()
	{
		_mediatorMock = new Mock<ISender>();
		_sut = new UserCreatedMessageConsumer(_mediatorMock.Object);
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
		await _sut.Consume(context.Object);

		// Assert
		_mediatorMock.Verify(
			a => a.Send(
				It.Is<CreateUserCommand>(b => b.UserId.Value.Equals(message.UserId) && b.Name == message.UserName),
				CancellationToken.None), Times.Once());
	}
}
