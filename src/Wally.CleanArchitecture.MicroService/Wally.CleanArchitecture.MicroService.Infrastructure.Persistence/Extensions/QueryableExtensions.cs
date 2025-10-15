using System;
using System.Linq;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

public static class QueryableExtensions
{
	public static IQueryable<TEntity> ApplyFilter<TEntity, TRequest>(this IQueryable<TEntity> query,
		IQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		return queryOptions.ApplyFilter<TEntity, TRequest>(query);
	}

	public static IQueryable<TEntity> ApplySearch<TEntity, TRequest>(this IQueryable<TEntity> query,
		IQueryOptions<TRequest>? queryOptions, Func<IQueryable<TEntity>, string, IQueryable<TEntity>> applySearch)
		where TRequest : class, IRequest
	{
		return queryOptions?.ApplySearch(query, applySearch) ?? query;
	}

	public static IQueryable<TEntity> ApplyOrderBy<TEntity, TRequest>(this IQueryable<TEntity> query,
		IQueryOptions<TRequest> queryOptions, Func<IQueryable<TEntity>, IQueryable<TEntity>> defaultOrderBy)
		where TRequest : class, IRequest
	{
		return queryOptions.ApplyOrderBy(query, defaultOrderBy);
	}

	public static IQueryable<TEntity> ApplyTop<TEntity, TRequest>(this IQueryable<TEntity> query,
		IQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		return queryOptions.Top == null ? query : query.Take(queryOptions.Top.Value);
	}

	public static IQueryable<TEntity> ApplySkip<TEntity, TRequest>(this IQueryable<TEntity> query,
		IQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		return queryOptions.Skip == null ? query : query.Skip(queryOptions.Skip.Value);
	}
}
