using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Extensions.ExpressionMapping;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.Domain.Abstractions;
using Wally.CleanArchitecture.Persistence.Exceptions;
using Wally.Lib.DDD.Abstractions.DomainModels;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

namespace Wally.CleanArchitecture.Persistence;

public abstract class Repository<TAggregateRoot> : IRepository<TAggregateRoot> where TAggregateRoot : AggregateRoot
{
	private readonly DbContext _context;
	private readonly IMapper _mapper;

	protected Repository(DbContext context, IMapper mapper)
	{
		_context = context;
		_mapper = mapper;
	}

	public Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
	{
		return GetReadOnlyEntitySet()
			.AnyAsync(a => a.Id == id, cancellationToken);
	}

	public Task<TAggregateRoot> GetAsync(Guid id, CancellationToken cancellationToken)
	{
		var task = GetReadWriteEntitySet()
			.SingleAsync(a => a.Id.Equals(id), cancellationToken);

		return MapExceptionAsync(task, id, cancellationToken);
	}

	public Task<TResult> GetAsync<TResult>(Guid id, CancellationToken cancellationToken) where TResult : IResponse
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));
		var task = _mapper.ProjectTo<TResult>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task, id);
	}

	public TAggregateRoot Add(TAggregateRoot aggregateRoot)
	{
		return _context.Add(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Remove(TAggregateRoot aggregateRoot)
	{
		return _context.Remove(aggregateRoot)
			.Entity;
	}

	public TAggregateRoot Update(TAggregateRoot aggregateRoot)
	{
		_context.Attach(aggregateRoot)
			.State = EntityState.Modified;

		return aggregateRoot;
	}

	[Obsolete("Workaround")]
	public TEntity Attach<TEntity>(TEntity entity) where TEntity : Entity
	{
		_context.Attach(entity)
			.State = EntityState.Unchanged;

		return entity;
	}

	public Task<PagedResponse<TResponse>> GetAsync
		<TRequest, TResponse>(ODataQueryOptions<TRequest> queryOptions, CancellationToken cancellationToken)
		where TRequest : class, IRequest where TResponse : class, IResponse
	{
		var query = GetReadOnlyEntitySet();
		return GetAsync<TRequest, TResponse>(query, queryOptions, cancellationToken);
	}

	protected IQueryable<TAggregateRoot> GetReadOnlyEntitySet()
	{
		return _context.Set<TAggregateRoot>()
			.AsNoTracking();
	}

	protected IQueryable<TEntity> GetReadOnlyEntitySet<TEntity>() where TEntity : Entity
	{
		return _context.Set<TEntity>()
			.AsNoTracking();
	}

	protected IQueryable<TAggregateRoot> GetReadWriteEntitySet()
	{
		return WithIncludes(_context.Set<TAggregateRoot>());
	}

	protected Task<TResponse> GetAsync<TResponse>(IQueryable<Entity> query, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleAsync(cancellationToken);

		return MapExceptionAsync(task);
	}

	protected Task<TResponse?> GetOrDefaultAsync
		<TResponse>(IQueryable<Entity> query, CancellationToken cancellationToken) where TResponse : IResponse
	{
		var task = _mapper.ProjectTo<TResponse>(query)
			.SingleOrDefaultAsync(cancellationToken);

		return MapExceptionAsync(task);
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

	protected async Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		IQueryable<TAggregateRoot> query,
		ODataQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken) where TRequest : class, IRequest where TResponse : class, IResponse
	{
		if (queryOptions.Filter != null)
		{
			var mappedQueryFunc = GetFilterExpression<TRequest>(queryOptions.Filter);

			query = query.Where(_mapper.MapExpression<Expression<Func<TAggregateRoot, bool>>>(mappedQueryFunc));
		}

		var allItems = _mapper.ProjectTo<TResponse>(query);
		var data = queryOptions.ApplyTo(_mapper.ProjectTo<TRequest>(query), AllowedQueryOptions.Filter);
		var items = await _mapper.ProjectTo<TResponse>(data)
			.ToArrayAsync(cancellationToken);

		var pageSize = queryOptions.Top?.Value ?? items.Length;

		return new PagedResponse<TResponse>(
			items,
			new PageInfoResponse(
				queryOptions.Skip?.Value > 0 && pageSize != 0 ? queryOptions.Skip.Value / pageSize : 0,
				pageSize,
				allItems.Count()));
	}

	protected virtual IQueryable<TAggregateRoot> WithIncludes(DbSet<TAggregateRoot> set)
	{
		return set;
	}

	private Task<TResult> MapExceptionAsync<TResult>(
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
							return tcs.TrySetException(new ResourceNotFoundException());
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
