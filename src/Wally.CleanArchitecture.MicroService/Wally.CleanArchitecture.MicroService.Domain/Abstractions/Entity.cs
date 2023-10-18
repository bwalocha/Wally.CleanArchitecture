using System;
using System.Collections.Generic;

using Wally.Lib.DDD.Abstractions.DomainEvents;
using Wally.Lib.DDD.Abstractions.Validators;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class Entity<TEntity, TKey>
	where TEntity : Entity<TEntity, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
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

	protected void AddDomainEvent(DomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void RemoveDomainEvent(DomainEvent domainEvent)
	{
		_domainEvents.Remove(domainEvent);
	}

	protected static void CheckRule(IBusinessRule rule)
	{
		if (rule.IsBroken())
		{
			throw new NotImplementedException();
		}
	}
}
