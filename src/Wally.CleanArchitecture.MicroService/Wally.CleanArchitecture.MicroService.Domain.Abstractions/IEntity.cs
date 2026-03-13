using System.Collections.Generic;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IEntity
{
	IReadOnlyCollection<DomainEvent> GetDomainEvents();

	void RemoveDomainEvent(DomainEvent domainEvent);
}
