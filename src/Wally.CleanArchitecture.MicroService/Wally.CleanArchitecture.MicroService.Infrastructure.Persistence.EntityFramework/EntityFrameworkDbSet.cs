using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.EntityFramework;

public class EntityFrameworkDbSet<TEntity> : IDbSet<TEntity>
	where TEntity : class
{
	private readonly DbSet<TEntity> _dbSet;

	public EntityFrameworkDbSet(DbSet<TEntity> dbSet)
	{
		_dbSet = dbSet;
	}

	public IQueryable<TEntity> AsNoTracking()
	{
		return _dbSet.AsNoTracking();
	}

	public IEnumerator<TEntity> GetEnumerator()
	{
		return _dbSet.AsEnumerable()
			.GetEnumerator();
	}

	IEnumerator IEnumerable.GetEnumerator()
	{
		return ((IEnumerable)_dbSet).GetEnumerator();
	}

	public Type ElementType => ((IQueryable)_dbSet).ElementType;

	public Expression Expression => ((IQueryable)_dbSet).Expression;

	public IQueryProvider Provider => ((IQueryable)_dbSet).Provider;

	public IAsyncEnumerator<TEntity> GetAsyncEnumerator(CancellationToken cancellationToken = default)
	{
		return _dbSet.AsAsyncEnumerable()
			.GetAsyncEnumerator(cancellationToken);
	}
}
