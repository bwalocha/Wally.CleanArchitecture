using System.Collections.Generic;
using Wally.Lib.DDD.Abstractions.DomainEvents;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IEntity
{
	IReadOnlyCollection<DomainEvent> GetDomainEvents();
	
	void RemoveDomainEvent(DomainEvent domainEvent);
}
