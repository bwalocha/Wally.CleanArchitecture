using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
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

	public Task<TResult> GetAsync<TResult>(Guid id, CancellationToken cancellationToken) where TResult : IResponse
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));
		var task = _mapper.ProjectTo<TResult>(query)
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

	protected IQueryable<TEntity> GetReadOnlyEntitySet()
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
	
	private static LambdaExpression GetOrderByExpression<T>(OrderByQueryOption order)
	{
		var enumerable = Enumerable.Empty<T>()
			.AsQueryable();
		enumerable = order.ApplyTo(enumerable, new ODataQuerySettings());
		var mce = (MethodCallExpression)enumerable.Expression;
		var quote = (UnaryExpression)mce.Arguments[1];
		return (LambdaExpression)quote.Operand;
	}
	
	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TEntity> query,
		ODataQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken) where TRequest : class, IRequest where TResponse : class, IResponse
	{
		if (queryOptions.Filter != null)
		{
			var queryFunc = GetFilterExpression<TRequest>(queryOptions.Filter);
			var mappedQueryFunc = _mapper.MapExpression<Expression<Func<TEntity, bool>>>(queryFunc); 
			
			query = query.Where(mappedQueryFunc);
		}

		var totalItems = await query.CountAsync(cancellationToken);

		if (queryOptions.OrderBy != null)
		{
			// TODO: support for multiple OrderBy, ThenBy, desc, asc combination
			var queryFunc = GetOrderByExpression<TRequest>(queryOptions.OrderBy);
			var destExpressionType = typeof(Expression<>).MakeGenericType(
				typeof(Func<,>).MakeGenericType(typeof(TEntity), queryFunc.ReturnType));
			var mappedQueryFunc = _mapper.MapExpression(queryFunc, queryFunc.GetType(), destExpressionType);
			MethodInfo[] methodInfo;
			if (queryOptions.OrderBy.OrderByClause.Direction == OrderByDirection.Ascending)
			{
				methodInfo = typeof(Queryable).GetMethods()
					.Where(a => a.Name == nameof(Queryable.OrderBy))
					.ToArray();
			}
			else
			{
				methodInfo = typeof(Queryable).GetMethods()
					.Where(a => a.Name == nameof(Queryable.OrderByDescending))
					.ToArray();
			}

			var mi = methodInfo.First()
				.MakeGenericMethod(typeof(TEntity), mappedQueryFunc.ReturnType); // TODO: get rid of First

			query = (IOrderedQueryable<TEntity>)mi.Invoke(null, new object[] { query, mappedQueryFunc })!;
		}
		else
		{
			query = query.OrderBy(a => a.Id);
		}
		
		if (queryOptions.Skip != null)
		{
			query = query.Skip(queryOptions.Skip.Value);
		}
		
		if (queryOptions.Top != null)
		{
			query = query.Take(queryOptions.Top.Value);
		}
		
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

	protected Task<TResult> MapExceptionAsync<TResult>(
		Task<TResult> task,
		Guid id = default,
		CancellationToken cancellationToken = default)
	{
		if (task == null)
		{
			throw new ArgumentNullException(nameof(task));
		}

		var tcs = new TaskCompletionSource<TResult>();

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
							var message = $"The '{typeof(TResult).Name}' could not be found";
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
