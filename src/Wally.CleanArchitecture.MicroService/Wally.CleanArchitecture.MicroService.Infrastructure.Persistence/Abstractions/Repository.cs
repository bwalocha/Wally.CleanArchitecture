using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public class Repository
	<TAggregateRoot, TKey> : ReadOnlyRepository<TAggregateRoot, TKey>, IRepository<TAggregateRoot, TKey>
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	protected Repository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}

	public Task<TAggregateRoot> GetAsync(TKey id, CancellationToken cancellationToken)
	{
		var task = GetReadWriteEntitySet()
			.SingleAsync(a => a.Id.Equals(id), cancellationToken);

		return MapExceptionAsync(task, id, cancellationToken);
	}

	public TAggregateRoot Add(TAggregateRoot aggregateRoot)
	{
		return DbContext.Add(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Remove(TAggregateRoot aggregateRoot)
	{
		return DbContext.Remove(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Update(TAggregateRoot aggregateRoot)
	{
		DbContext.Attach(aggregateRoot)
			.State = EntityState.Modified;

		return aggregateRoot;
	}

	protected IQueryable<TAggregateRoot> GetReadWriteEntitySet()
	{
		return WithIncludes(DbContext.Set<TAggregateRoot>());
	}

	protected virtual IQueryable<TAggregateRoot> WithIncludes(DbSet<TAggregateRoot> set)
	{
		return set;
	}
}
