using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;

namespace Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Abstractions;

public class ReadOnlyRepository<TEntity, TStronglyTypedId> : IReadOnlyRepository<TEntity, TStronglyTypedId>
	where TEntity : Entity<TEntity, TStronglyTypedId>
	where TStronglyTypedId : notnull, new()
{
	private readonly IMapper _mapper;
	protected readonly DbContext DbContext;

	protected ReadOnlyRepository(DbContext context, IMapper mapper)
	{
		DbContext = context;
		_mapper = mapper;
	}

	public Task<bool> ExistsAsync(TStronglyTypedId id, CancellationToken cancellationToken)
	{
		return GetReadOnlyEntitySet()
			.AnyAsync(a => a.Id.Equals(id), cancellationToken);
	}

	public async Task<TResult> GetAsync<TResult>(TStronglyTypedId id, CancellationToken cancellationToken)
		where TResult : IResult
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));

		return await _mapper.ProjectTo<TResult>(query)
				.SingleOrDefaultAsync(cancellationToken)
		?? throw new ResourceNotFoundException<TResult>(id);
	}

	public Task<PagedResult<TResult>> GetAsync<TRequest, TResult>(
		IQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResult : class, IResult // TODO: struct?
	{
		var query = GetReadOnlyEntitySet();

		return GetAsync<TRequest, TResult>(query, queryOptions, cancellationToken);
	}

	protected async Task<TResponse> GetAsync<TResponse>(IQueryable<TEntity> query, CancellationToken cancellationToken)
		where TResponse : IResult
	{
		return await _mapper.ProjectTo<TResponse>(query)
				.FirstOrDefaultAsync(cancellationToken)
		?? throw new ResourceNotFoundException<TResponse>();
	}

	protected async Task<PagedResult<TResult>> GetAsync<TRequest, TResult>(
		IQueryable<TEntity> query,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResult : class, IResult
	{
		var totalItems = await query.CountAsync(cancellationToken);
		var items = await _mapper.ProjectTo<TResult>(query)
			.ToArrayAsync(cancellationToken);

		return new PagedResult<TResult>(items, new PageInfoResult(0, items.Length, totalItems));
	}

	protected async Task<PagedResult<TResult>> GetAsync<TRequest, TResult>(
		IQueryable<TEntity> query,
		IQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResult : class, IResult // TODO: struct?
	{
		query = query
			.ApplyFilter(queryOptions)
			.ApplySearch(queryOptions, ApplySearch);

		var totalItems = query.Provider is IAsyncQueryProvider ? await query.CountAsync(cancellationToken) : query.Count();

		query = query
			.ApplyOrderBy(queryOptions, ApplyDefaultOrderBy)
			.ApplySkip(queryOptions)
			.ApplyTop(queryOptions);
		
		var items = query.Provider is IAsyncQueryProvider ? await _mapper.ProjectTo<TResult>(query)
			.ToArrayAsync(cancellationToken) : _mapper.ProjectTo<TResult>(query).ToArray();

		var pageSize = queryOptions.Top ?? items.Length;

		return new PagedResult<TResult>(
			items,
			new PageInfoResult(
				queryOptions.Skip > 0 && pageSize != 0 ? queryOptions.Skip.Value / pageSize : 0,
				pageSize,
				totalItems));
	}

	protected async Task<TResult?> GetOrDefaultAsync<TResult>(
		IQueryable<TEntity> query,
		CancellationToken cancellationToken)
		where TResult : IResult
	{
		return await _mapper.ProjectTo<TResult>(query)
				.SingleOrDefaultAsync(cancellationToken)
		?? throw new ResourceNotFoundException<TResult>();
	}

	protected virtual IQueryable<TEntity> GetReadOnlyEntitySet()
	{
		return DbContext.Set<TEntity>()
			.AsNoTracking();
	}

	protected virtual IQueryable<TEntity> ApplySearch(IQueryable<TEntity> query, string term)
	{
		throw new NotSupportedException($"Search on {typeof(TEntity).Name} is not supported");
	}

	protected virtual IQueryable<TEntity> ApplyDefaultOrderBy(IQueryable<TEntity> query)
	{
		return query.OrderBy(a => a.Id);
	}
}
