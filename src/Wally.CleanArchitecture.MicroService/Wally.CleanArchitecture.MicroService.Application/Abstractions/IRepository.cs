using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IRepository<TAggregateRoot, TStronglyTypedId> : IReadOnlyRepository<TAggregateRoot, TStronglyTypedId>
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TStronglyTypedId>
	where TStronglyTypedId : new()
{
	Task<TAggregateRoot> GetAsync(TStronglyTypedId id, CancellationToken cancellationToken);

	TAggregateRoot Add(TAggregateRoot aggregateRoot);

	TAggregateRoot Update(TAggregateRoot aggregateRoot);

	TAggregateRoot Remove(TAggregateRoot aggregateRoot);
}
