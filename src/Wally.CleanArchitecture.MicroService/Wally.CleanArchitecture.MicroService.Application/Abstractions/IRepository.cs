using System;
using System.Threading;
using System.Threading.Tasks;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IRepository<TAggregateRoot, in TKey> : IReadOnlyRepository<TAggregateRoot, TKey>
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	Task<TAggregateRoot> GetAsync(TKey id, CancellationToken cancellationToken);

	TAggregateRoot Add(TAggregateRoot aggregateRoot);

	TAggregateRoot Update(TAggregateRoot aggregateRoot);

	TAggregateRoot Remove(TAggregateRoot aggregateRoot);
}
