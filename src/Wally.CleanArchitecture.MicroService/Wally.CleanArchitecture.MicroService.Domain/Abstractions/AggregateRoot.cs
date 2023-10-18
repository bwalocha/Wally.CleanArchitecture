using System;

using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public abstract class AggregateRoot<TAggregateRoot, TKey> : Entity<TAggregateRoot, TKey>, IAggregateRoot
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	protected AggregateRoot()
	{
	}

	protected AggregateRoot(TKey id)
		: base(id)
	{
	}

	public DateTimeOffset CreatedAt { get; private set; }

	public UserId CreatedById { get; private set; } = null!;

	public DateTimeOffset? ModifiedAt { get; private set; }

	public UserId? ModifiedById { get; private set; }
}
