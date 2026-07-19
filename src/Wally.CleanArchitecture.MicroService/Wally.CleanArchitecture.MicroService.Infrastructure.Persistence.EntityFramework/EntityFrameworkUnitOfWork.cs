using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.EntityFramework;

public sealed class EntityFrameworkUnitOfWork : IUnitOfWork
{
	private readonly Microsoft.EntityFrameworkCore.DbContext _dbContext;

	public EntityFrameworkUnitOfWork(Microsoft.EntityFrameworkCore.DbContext dbContext)
	{
		_dbContext = dbContext;
	}

	// public override ChangeTracker ChangeTracker => _dbContext.ChangeTracker;
	//
	// public override DatabaseFacade Database => _dbContext.Database;
	//
	// public override EntityEntry Entry(object entity)
	// {
	// 	return _dbContext.Entry(entity);
	// }

	public TEntity Add<TEntity>(TEntity entity)
		where TEntity : class
	{
		return _dbContext.Add(entity).Entity;
	}

	public async Task AddRangeAsync(params object[] entities)
	{
		await _dbContext.AddRangeAsync(entities);
	}

	public TEntity Update<TEntity>(TEntity entity)
		where TEntity : class
	{
		var entry = _dbContext.Attach(entity);
		entry.State = EntityState.Modified;

		return entry.Entity;
	}

	public TEntity Remove<TEntity>(TEntity entity)
		where TEntity : class
	{
		return _dbContext.Remove(entity).Entity;
	}

	public void RemoveRange(params object[] entities)
	{
		_dbContext.RemoveRange((IEnumerable<object>)entities);
	}

	public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class
	{
		_dbContext.RemoveRange(entities);
	}

	public IDbSet<TEntity> Set<TEntity>()
		where TEntity : class
	{
		return new EntityFrameworkDbSet<TEntity>(_dbContext.Set<TEntity>());
	}

	public int SaveChanges()
	{
		return _dbContext.SaveChanges();
	}

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
	{
		return _dbContext.SaveChangesAsync(cancellationToken);
	}
}
