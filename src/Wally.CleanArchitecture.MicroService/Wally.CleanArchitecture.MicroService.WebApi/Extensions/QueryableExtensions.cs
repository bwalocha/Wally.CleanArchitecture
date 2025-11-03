using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.OData.UriParser;
using Wally.CleanArchitecture.MicroService.WebApi.Abstractions;

// using Wally.CleanArchitecture.MicroService.Application.Abstractions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Extensions;

public static class QueryableExtensions
{
	public static IQueryable<TEntity> ApplyFilter<TEntity, TRequest>(this IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions, IMapper mapper)
		where TRequest : /*class,*/ IRequest
	{
		if (queryOptions.Filter == null)
		{
			return query;
		}

		var queryFunc = GetFilterExpression<TRequest>(queryOptions.Filter);
		var mappedQueryFunc = mapper.MapExpression<Expression<Func<TEntity, bool>>>(queryFunc);

		return query.Where(mappedQueryFunc);
	}

	public static IQueryable<TEntity> ApplySearch<TEntity, TRequest>(this IQueryable<TEntity> query,
		ODataQueryOptions<TRequest>? queryOptions, Func<IQueryable<TEntity>, string, IQueryable<TEntity>> applySearch)
		where TRequest : /*class,*/ IRequest
	{
		if (queryOptions?.Search == null)
		{
			return query;
		}

		if (queryOptions.Search.SearchClause.Expression is not SearchTermNode searchTermNode)
		{
			throw new NotSupportedException($"Advanced Search on {typeof(TEntity).Name} is not supported");
		}

		return applySearch(query, searchTermNode.Text);
	}

	public static IQueryable<TEntity> ApplyOrderBy<TEntity, TRequest>(this IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions, Func<IQueryable<TEntity>, IQueryable<TEntity>> applyDefaultOrderBy,
		IMapper mapper)
		where TRequest : /*class,*/ IRequest
	{
		if (queryOptions.OrderBy == null)
		{
			return applyDefaultOrderBy(query);
		}

		var queryFuncs = GetOrderByExpression<TRequest>(queryOptions.OrderBy);

		foreach (var queryFunc in queryFuncs.Select((a, i) => new { Expression = a, Index = i, }))
		{
			var destExpressionType = typeof(Expression<>).MakeGenericType(
				typeof(Func<,>).MakeGenericType(typeof(TEntity), queryFunc.Expression.ReturnType));
			var mappedQueryFunc = mapper.MapExpression(
				queryFunc.Expression,
				queryFunc.Expression.GetType(),
				destExpressionType);
			string orderMethodName;
			var direction = queryOptions.OrderBy.OrderByNodes[queryFunc.Index].Direction;

			if (direction == OrderByDirection.Ascending)
			{
				orderMethodName = queryFunc.Index == 0 ? nameof(Queryable.OrderBy) : nameof(Queryable.ThenBy);
			}
			else
			{
				orderMethodName = queryFunc.Index == 0
					? nameof(Queryable.OrderByDescending)
					: nameof(Queryable.ThenByDescending);
			}

			var methodInfo = typeof(Queryable).GetMethods()
				.Where(a => a.Name == orderMethodName)
				.ToArray();
			var mi = methodInfo[0]
				.MakeGenericMethod(typeof(TEntity), mappedQueryFunc.ReturnType);

			query = (IOrderedQueryable<TEntity>)mi.Invoke(null, [query, mappedQueryFunc,]) !;
		}

		return query;
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

	private static IEnumerable<LambdaExpression> GetOrderByExpression<T>(OrderByQueryOption order)
	{
		var enumerable = Enumerable.Empty<T>()
			.AsQueryable();
		enumerable = order.ApplyTo(enumerable, new ODataQuerySettings());
		var mce = (MethodCallExpression)enumerable.Expression;

		foreach (var argument in mce.Arguments)
		{
			switch (argument)
			{
				case UnaryExpression unaryExpression:
					yield return (LambdaExpression)unaryExpression.Operand;
					break;
				case MethodCallExpression callExpression:
					yield return (LambdaExpression)((UnaryExpression)callExpression.Arguments[1]).Operand;
					break;
				default:
					continue;
			}
		}
	}

	public static IQueryable<TEntity> ApplyTop<TEntity, TRequest>(this IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions)
		where TRequest : /*class,*/ IRequest
	{
		if (queryOptions.Top == null)
		{
			return query;
		}

		return query.Take(queryOptions.Top.Value);
	}

	public static IQueryable<TEntity> ApplySkip<TEntity, TRequest>(this IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions)
		where TRequest : /*class,*/ IRequest
	{
		if (queryOptions.Skip == null)
		{
			return query;
		}

		return query.Skip(queryOptions.Skip.Value);
	}
}
