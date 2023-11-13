using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;

using Wally.CleanArchitecture.MicroService.Domain.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public class Repository
	<TAggregateRoot, TKey> : ReadOnlyRepository<TAggregateRoot, TKey>, IRepository<TAggregateRoot, TKey>
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	private readonly DbContext _context;

	protected Repository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
		_context = context;
	}

	public Task<TAggregateRoot> GetAsync(TKey id, CancellationToken cancellationToken)
	{
		var task = GetReadWriteEntitySet()
			.SingleAsync(a => a.Id.Equals(id), cancellationToken);

		return MapExceptionAsync(task, id, cancellationToken);
	}

	public TAggregateRoot Add(TAggregateRoot aggregateRoot)
	{
		return _context.Add(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Remove(TAggregateRoot aggregateRoot)
	{
		return _context.Remove(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Update(TAggregateRoot aggregateRoot)
	{
		_context.Attach(aggregateRoot)
			.State = EntityState.Modified;

		return aggregateRoot;
	}

	/*[Obsolete("Workaround")]
	public TEntity Attach<TEntity>(TEntity entity) where TEntity : Wally.Lib.DDD.Abstractions.DomainModels.Entity
	{
		_context.Attach(entity)
			.State = EntityState.Unchanged;

		return entity;
	}*/

	protected IQueryable<TAggregateRoot> GetReadWriteEntitySet()
	{
		return WithIncludes(_context.Set<TAggregateRoot>());
	}

	private static Expression<Func<T, bool>> GetFilterExpression<T>(FilterQueryOption filter)
	{
		var enumerable = Enumerable.Empty<T>()
			.AsQueryable();
		enumerable = (IQueryable<T>)filter.ApplyTo(enumerable, new ODataQuerySettings());
		var mce = (MethodCallExpression)enumerable.Expression;
		var quote = (UnaryExpression)mce.Arguments[1];
		return (Expression<Func<T, bool>>)quote.Operand;
	}

	protected virtual IQueryable<TAggregateRoot> WithIncludes(DbSet<TAggregateRoot> set)
	{
		return set;
	}
}
