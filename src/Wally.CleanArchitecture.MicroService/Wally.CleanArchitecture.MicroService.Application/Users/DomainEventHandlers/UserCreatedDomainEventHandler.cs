using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Application.Contracts.Users.Responses;
using Wally.CleanArchitecture.MicroService.Application.Messages.Users;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Application.Users.DomainEventHandlers;

public class UserCreatedDomainEventHandler : IDomainEventHandler<UserCreatedDomainEvent>
{
	private readonly IBus _bus;
	private readonly IUserReadOnlyRepository _userRepository;

	public UserCreatedDomainEventHandler(IBus bus, IUserReadOnlyRepository userRepository)
	{
		_bus = bus;
		_userRepository = userRepository;
	}

	public async Task HandleAsync(UserCreatedDomainEvent domainEvent, CancellationToken cancellationToken)
	{
		var model = await _userRepository.GetAsync<GetUserResponse>(domainEvent.Id, cancellationToken);
		
		var message = new UserCreatedMessage(domainEvent.Id.Value, model.Name);

		await _bus.Publish(message, cancellationToken);
	}
}
