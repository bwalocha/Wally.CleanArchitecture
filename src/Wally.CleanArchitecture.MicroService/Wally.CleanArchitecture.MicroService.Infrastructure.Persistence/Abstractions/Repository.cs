using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public class Repository<TAggregateRoot, TStronglyTypedId> : ReadOnlyRepository<TAggregateRoot, TStronglyTypedId>,
	IRepository<TAggregateRoot, TStronglyTypedId>
	where TAggregateRoot : AggregateRoot<TAggregateRoot, TStronglyTypedId>
	where TStronglyTypedId : notnull, new()
{
	protected Repository(DbContext context, IMapper mapper)
		: base(context, mapper)
	{
	}
	
	public async Task<TAggregateRoot> GetAsync(TStronglyTypedId id, CancellationToken cancellationToken)
	{
		return await GetReadWriteEntitySet()
				.SingleOrDefaultAsync(a => a.Id.Equals(id), cancellationToken)
		?? throw new ResourceNotFoundException<TAggregateRoot>(id);
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
