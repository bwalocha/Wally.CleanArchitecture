using System.Collections.Generic;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public interface IDbSet<out TEntity> : IOrderedQueryable<TEntity>, IAsyncEnumerable<TEntity>
	where TEntity : class
{
}
