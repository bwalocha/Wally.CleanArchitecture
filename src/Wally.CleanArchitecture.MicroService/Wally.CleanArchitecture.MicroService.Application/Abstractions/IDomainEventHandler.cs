using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IDomainEventHandler<in TDomainEvent>
	where TDomainEvent : DomainEvent
{
	Task HandleAsync(TDomainEvent domainEvent, CancellationToken cancellationToken);
}
