using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public interface IUnitOfWork
{
	// public ChangeTracker ChangeTracker { get; }

	// public DatabaseFacade Database { get; }

	// public EntityEntry Entry(object entity);

	public TEntity Add<TEntity>(TEntity entity)
		where TEntity : class;

	public Task AddRangeAsync(params object[] entities);

	public TEntity Update<TEntity>(TEntity entity)
		where TEntity : class;

	public TEntity Remove<TEntity>(TEntity entity)
		where TEntity : class;

	public void RemoveRange(params object[] entities);

	public void RemoveRange<TEntity>(IEnumerable<TEntity> entities)
		where TEntity : class;

	public IDbSet<TEntity> Set<TEntity>()
		where TEntity : class;

	public int SaveChanges();

	public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
