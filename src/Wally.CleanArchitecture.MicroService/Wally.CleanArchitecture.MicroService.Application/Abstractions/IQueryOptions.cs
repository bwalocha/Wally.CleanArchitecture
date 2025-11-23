using System;
using System.Linq;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IQueryOptions<TRequest>
	// where TRequest : IRequest
{
	int? Skip { get; }

	int? Top { get; }

	IQueryable<TEntity> ApplyFilter<TEntity, TResult>(IQueryable<TEntity> query /*, IMapper mapper*/)
		// where TEntity : Entity
		;

	IQueryable<TEntity> ApplySearch<TEntity>(IQueryable<TEntity> query,
			Func<IQueryable<TEntity>, string, IQueryable<TEntity>> search)
		// where TEntity : Entity
		;

	IQueryable<TEntity> ApplyOrderBy<TEntity>(IQueryable<TEntity> query,
			Func<IQueryable<TEntity>, IQueryable<TEntity>> applyDefaultOrderBy)
		// where TEntity : Entity
		;

	IQueryable<TEntity> ApplySkip<TEntity>(IQueryable<TEntity> query)
		// where TEntity : Entity
		;

	IQueryable<TEntity> ApplyTop<TEntity>(IQueryable<TEntity> query)
		// 	where TEntity : Entity
		;
}
