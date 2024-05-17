using System;
using Wally.CleanArchitecture.MicroService.Domain.Users;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public class AggregateRoot<TAggregateRoot, TStronglyTypedId>
	: Entity<TAggregateRoot, TStronglyTypedId>, IAggregateRoot
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TStronglyTypedId>
	where TStronglyTypedId : new()
{
	protected AggregateRoot()
	{
	}
	
	protected AggregateRoot(TStronglyTypedId id)
		: base(id)
	{
	}
	
	public DateTimeOffset CreatedAt { get; private set; }
	
	public UserId CreatedById { get; private set; } = null!;
	
	public DateTimeOffset? ModifiedAt { get; private set; }
	
	public UserId? ModifiedById { get; private set; }
}
