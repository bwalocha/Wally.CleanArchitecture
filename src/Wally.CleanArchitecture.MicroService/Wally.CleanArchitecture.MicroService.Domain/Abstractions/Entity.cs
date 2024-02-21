using System;
using System.Collections.Generic;
using Wally.Lib.DDD.Abstractions.DomainEvents;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class Entity<TEntity, TKey> : IEntity
	where TEntity : Entity<TEntity, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, new()
{
	private readonly List<DomainEvent> _domainEvents = new();

	protected Entity()
		: this(new TKey())
	{
	}

	protected Entity(TKey id)
	{
		Id = id;
	}

	public TKey Id { get; private set; }

	public IReadOnlyCollection<DomainEvent> GetDomainEvents()
	{
		return _domainEvents.AsReadOnly();
	}

	public void RemoveDomainEvent(DomainEvent domainEvent)
	{
		_domainEvents.Remove(domainEvent);
	}

	protected void AddDomainEvent(DomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}
}
