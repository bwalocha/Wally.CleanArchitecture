using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Microsoft.OData.UriParser;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

// Right now the ReadOnlyRepository uses EF for obtaining data form the Database.
// We can consider to use Dapper or even ADO .Net if the performance in not efficient.
public class ReadOnlyRepository<TEntity, TKey> : IReadOnlyRepository<TEntity, TKey>
	where TEntity : Entity<TEntity, TKey>
	where TKey : notnull, IComparable<TKey>, IEquatable<TKey>, IStronglyTypedId<TKey, Guid>, new()
{
	private readonly IMapper _mapper;
	protected readonly DbContext DbContext;

	protected ReadOnlyRepository(DbContext context, IMapper mapper)
	{
		DbContext = context;
		_mapper = mapper;
	}

	public Task<bool> ExistsAsync(TKey id, CancellationToken cancellationToken)
	{
		return GetReadOnlyEntitySet()
			.AnyAsync(a => a.Id.Equals(id), cancellationToken);
	}

	public Task<TResponse> GetAsync<TResponse>(TKey id, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task, id, cancellationToken);
	}

	public Task<PagedResponse<TResponse>> GetAsync
		<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResponse : class, IResponse
	{
		var query = GetReadOnlyEntitySet();
		return GetAsync<TRequest, TResponse>(query, queryOptions, cancellationToken);
	}

	protected Task<TResponse> GetAsync<TResponse>(IQueryable<TEntity> query, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task, cancellationToken: cancellationToken);
	}

	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TEntity> query,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResponse : class, IResponse
	{
		var totalItems = await query.CountAsync(cancellationToken);
		var items = await _mapper.ProjectTo<TResponse>(query)
			.ToArrayAsync(cancellationToken);

		return new PagedResponse<TResponse>(items, new PageInfoResponse(0, items.Length, totalItems));
	}

	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResponse : class, IResponse
	{
		query = ApplyFilter(query, queryOptions);
		query = ApplySearch(query, queryOptions);

		var totalItems = await query.CountAsync(cancellationToken);

		query = ApplyOrderBy(query, queryOptions);
		query = ApplySkip(query, queryOptions);
		query = ApplyTop(query, queryOptions);

		var items = await _mapper.ProjectTo<TResponse>(query)
			.ToArrayAsync(cancellationToken);

		var pageSize = queryOptions.Top?.Value ?? items.Length;

		return new PagedResponse<TResponse>(
			items,
			new PageInfoResponse(
				queryOptions.Skip?.Value > 0 && pageSize != 0 ? queryOptions.Skip.Value / pageSize : 0,
				pageSize,
				totalItems));
	}

	protected Task<TResponse?> GetOrDefaultAsync
		<TResponse>(IQueryable<TEntity> query, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleOrDefaultAsync(cancellationToken);

		return MapExceptionAsync(task, cancellationToken: cancellationToken);
	}

	protected virtual IQueryable<TEntity> GetReadOnlyEntitySet()
	{
		return DbContext.Set<TEntity>()
			.AsNoTracking();
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

	private static IQueryable<TEntity> ApplyTop<TRequest>(IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		if (queryOptions.Top == null)
		{
			return query;
		}

		return query.Take(queryOptions.Top.Value);
	}

	private static IQueryable<TEntity> ApplySkip<TRequest>(IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		if (queryOptions.Skip == null)
		{
			return query;
		}

		return query.Skip(queryOptions.Skip.Value);
	}

	private IQueryable<TEntity> ApplyOrderBy<TRequest>(IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		if (queryOptions.OrderBy == null)
		{
			return ApplyDefaultOrderBy(query);
		}

		var queryFuncs = GetOrderByExpression<TRequest>(queryOptions.OrderBy);

		foreach (var queryFunc in queryFuncs.Select((a, i) => new { Expression = a, Index = i, }))
		{
			var destExpressionType = typeof(Expression<>).MakeGenericType(
				typeof(Func<,>).MakeGenericType(typeof(TEntity), queryFunc.Expression.ReturnType));
			var mappedQueryFunc = _mapper.MapExpression(
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

			query = (IOrderedQueryable<TEntity>)mi.Invoke(null, new object[] { query, mappedQueryFunc, }) !;
		}

		return query;
	}

	private IQueryable<TEntity> ApplyFilter
		<TRequest>(IQueryable<TEntity> query, ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest
	{
		if (queryOptions.Filter == null)
		{
			return query;
		}

		var queryFunc = GetFilterExpression<TRequest>(queryOptions.Filter);
		var mappedQueryFunc = _mapper.MapExpression<Expression<Func<TEntity, bool>>>(queryFunc);

		return query.Where(mappedQueryFunc);
	}

	private IQueryable<TEntity> ApplySearch
		<TRequest>(IQueryable<TEntity> query, ODataQueryOptions<TRequest>? queryOptions)
		where TRequest : class, IRequest
	{
		if (queryOptions?.Search == null)
		{
			return query;
		}

		if (queryOptions.Search.SearchClause.Expression is not SearchTermNode searchTermNode)
		{
			throw new NotSupportedException($"Advanced Search on {typeof(TEntity).Name} is not supported");
		}

		return ApplySearch(query, searchTermNode.Text);
	}

	protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string term)
	{
		throw new NotSupportedException($"Search on {typeof(TEntity).Name} is not supported");
	}

	protected virtual IQueryable<TEntity> ApplyDefaultOrderBy(IQueryable<TEntity> query)
	{
		return query.OrderBy(a => a.Id);
	}

	protected static Task<TResponse> MapExceptionAsync<TResponse>(
		Task<TResponse> task,
		TKey? id = default,
		CancellationToken cancellationToken = default)
	{
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}

		var tcs = new TaskCompletionSource<TResponse>();

		task.ContinueWith(
			t => tcs.TrySetCanceled(),
			cancellationToken,
			TaskContinuationOptions.OnlyOnCanceled,
			TaskScheduler.Default);
		task.ContinueWith(
			t => tcs.TrySetResult(t.Result),
			cancellationToken,
			TaskContinuationOptions.OnlyOnRanToCompletion,
			TaskScheduler.Default);
		task.ContinueWith(
			t =>
			{
				if (t.Exception!.GetBaseException() is not InvalidOperationException exception)
				{
					return tcs.TrySetException(t.Exception);
				}

				switch (exception.TargetSite!.Name)
				{
					case "ThrowNoElementsException":
					case "MoveNext":
						var message = $"The '{typeof(TResponse).Name}' could not be found";
						if (id is not null)
						{
							message += $" for Id: '{id}'";
						}

						return tcs.TrySetException(new ResourceNotFoundException(message, exception));
					default:
						return tcs.TrySetException(t.Exception);
				}
			},
			cancellationToken,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.Default);

		return tcs.Task;
	}
}
