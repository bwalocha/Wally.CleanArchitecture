using System;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

// TODO: add ModifiedAt, ModifiedBy, CreatedAt, CreatedBy
public abstract class AggregateRoot : Lib.DDD.Abstractions.DomainModels.AggregateRoot
{
	protected AggregateRoot()
	{
	}

	protected AggregateRoot(Guid id)
		: base(id)
	{
	}
}
