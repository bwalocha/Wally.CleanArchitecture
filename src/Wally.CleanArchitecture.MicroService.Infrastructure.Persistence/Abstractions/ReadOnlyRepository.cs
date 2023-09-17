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
using Wally.Lib.DDD.Abstractions.DomainModels;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

// Right now the ReadOnlyRepository uses EF for obtaining data form the Database.
// We can consider to use Dapper or even ADO .Net if the performance in not efficient.
public abstract class ReadOnlyRepository<TEntity> : IReadOnlyRepository<TEntity> where TEntity : Entity
{
	private readonly DbContext _context;
	private readonly IMapper _mapper;

	protected ReadOnlyRepository(DbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
	{
		return GetReadOnlyEntitySet()
			.AnyAsync(a => a.Id.Equals(id), cancellationToken);
	}

	public Task<TResponse> GetAsync<TResponse>(Guid id, CancellationToken cancellationToken) where TResponse : IResponse
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task, id, cancellationToken);
	}

	public Task<PagedResponse<TResponse>> GetAsync
		<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest where TResponse : class, IResponse
	{
		var query = GetReadOnlyEntitySet();
		return GetAsync<TRequest, TResponse>(query, queryOptions, cancellationToken);
	}

	protected virtual IQueryable<TEntity> GetReadOnlyEntitySet()
	{
		return _context.Set<TEntity>()
			.AsNoTracking();
	}

	protected Task<TResponse> GetAsync<TResponse>(IQueryable<Entity> query, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task, cancellationToken: cancellationToken);
	}

	protected Task<TResponse?> GetOrDefaultAsync
		<TResponse>(IQueryable<Entity> query, CancellationToken cancellationToken) where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleOrDefaultAsync(cancellationToken);

		return MapExceptionAsync(task, cancellationToken: cancellationToken);
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

				// TODO:
				/*else
				{
					throw new NotSupportedException($"The Sort Order '{order.RawValue}' is not supported");
				}*/
			}
		}
	}

	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TEntity> query,
		CancellationToken cancellationToken) where TRequest : class, IRequest where TResponse : class, IResponse
	{
		var totalItems = await query.CountAsync(cancellationToken);
		var items = await _mapper.ProjectTo<TResponse>(query)
			.ToArrayAsync(cancellationToken);

		return new PagedResponse<TResponse>(items, new PageInfoResponse(0, items.Length, totalItems));
	}

	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken) where TRequest : class, IRequest where TResponse : class, IResponse
	{
		query = ApplyFilter<TRequest, TResponse>(query, queryOptions);
		query = ApplySearch<TRequest, TResponse>(query, queryOptions);

		var totalItems = await query.CountAsync(cancellationToken);

		query = ApplyOrderBy<TRequest, TResponse>(query, queryOptions);
		query = ApplySkip<TRequest, TResponse>(query, queryOptions);
		query = ApplyTop<TRequest, TResponse>(query, queryOptions);

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

	private static IQueryable<TEntity> ApplyTop
		<TRequest, TResponse>(IQueryable<TEntity> query, ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest where TResponse : class, IResponse
	{
		if (queryOptions.Top == null)
		{
			return query;
		}

		return query.Take(queryOptions.Top.Value);
	}

	private static IQueryable<TEntity> ApplySkip
		<TRequest, TResponse>(IQueryable<TEntity> query, ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest where TResponse : class, IResponse
	{
		if (queryOptions.Skip == null)
		{
			return query;
		}

		return query.Skip(queryOptions.Skip.Value);
	}

	private IQueryable<TEntity> ApplyOrderBy
		<TRequest, TResponse>(IQueryable<TEntity> query, ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest where TResponse : class, IResponse
	{
		if (queryOptions.OrderBy != null)
		{
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
				if (queryOptions.OrderBy.OrderByNodes[queryFunc.Index].Direction == OrderByDirection.Ascending)
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
				var mi = methodInfo.First()
					.MakeGenericMethod(typeof(TEntity), mappedQueryFunc.ReturnType); // TODO: get rid of First

				query = (IOrderedQueryable<TEntity>)mi.Invoke(null, new object[] { query, mappedQueryFunc, }) !;
			}
		}
		else
		{
			query = ApplyDefaultOrderBy(query);
		}

		return query;
	}

	private IQueryable<TEntity> ApplyFilter
		<TRequest, TResponse>(IQueryable<TEntity> query, ODataQueryOptions<TRequest> queryOptions)
		where TRequest : class, IRequest where TResponse : class, IResponse
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
		<TRequest, TResponse>(IQueryable<TEntity> query, ODataQueryOptions<TRequest>? queryOptions)
		where TRequest : class, IRequest where TResponse : class, IResponse
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

	protected Task<TResponse> MapExceptionAsync<TResponse>(
		Task<TResponse> task,
		Guid id = default,
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
				if (t.Exception!.GetBaseException() is InvalidOperationException exception)
				{
					switch (exception.TargetSite!.Name)
					{
						case "ThrowNoElementsException":
						case "MoveNext":
							var message = $"The '{typeof(TResponse).Name}' could not be found";
							if (id != Guid.Empty)
							{
								message += $" for Id: '{id}'";
							}

							return tcs.TrySetException(new ResourceNotFoundException(message, exception));
					}
				}

				return tcs.TrySetException(t.Exception);
			},
			cancellationToken,
			TaskContinuationOptions.OnlyOnFaulted,
			TaskScheduler.Default);

		return tcs.Task;
	}
}
