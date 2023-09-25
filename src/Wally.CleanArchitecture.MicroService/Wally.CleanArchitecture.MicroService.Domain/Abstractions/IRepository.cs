using System;
using System.Threading;
using System.Threading.Tasks;

using Wally.Lib.DDD.Abstractions.DomainModels;

namespace Wally.CleanArchitecture.MicroService.Domain.Abstractions;

public interface IRepository<TAggregateRoot> : IReadOnlyRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
{
	Task<TAggregateRoot> GetAsync(Guid id, CancellationToken cancellationToken);

	TAggregateRoot Add(TAggregateRoot aggregateRoot);

	TAggregateRoot Update(TAggregateRoot aggregateRoot);

	TAggregateRoot Remove(TAggregateRoot aggregateRoot);

	TEntity Attach<TEntity>(TEntity entity) where TEntity : Entity;
}
