using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.EntityFrameworkCore;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.Domain.Abstractions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Exceptions;
using Wally.CleanArchitecture.MicroService.Infrastructure.Persistence.Extensions;
using Wally.Lib.DDD.Abstractions.Requests;
using Wally.Lib.DDD.Abstractions.Responses;

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

	public async Task<TResponse> GetAsync<TResponse>(TStronglyTypedId id, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		var query = GetReadOnlyEntitySet()
			.Where(a => a.Id.Equals(id));
		
		return await _mapper.ProjectTo<TResponse>(query)
			.SingleOrDefaultAsync(cancellationToken)
		?? throw new ResourceNotFoundException<TResponse>(id);
	}

	public Task<PagedResponse<TResponse>> GetAsync<TRequest, TResponse>(
		ODataQueryOptions<TRequest> queryOptions,
		CancellationToken cancellationToken)
		where TRequest : class, IRequest
		where TResponse : class, IResponse
	{
		var query = GetReadOnlyEntitySet();
		
		return GetAsync<TRequest, TResponse>(query, queryOptions, cancellationToken);
	}

	protected async Task<TResponse> GetAsync<TResponse>(IQueryable<TEntity> query, CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		return await _mapper.ProjectTo<TResponse>(query)
			.FirstOrDefaultAsync(cancellationToken)
		?? throw new ResourceNotFoundException<TResponse>();
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
		query = query.ApplyFilter(queryOptions, _mapper)
			.ApplySearch(queryOptions, ApplySearch);

		var totalItems = await query.CountAsync(cancellationToken);

		query = query.ApplyOrderBy(queryOptions, ApplyDefaultOrderBy, _mapper)
			.ApplySkip(queryOptions)
			.ApplyTop(queryOptions);

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

	protected async Task<TResponse?> GetOrDefaultAsync<TResponse>(
		IQueryable<TEntity> query,
		CancellationToken cancellationToken)
		where TResponse : IResponse
	{
		return await _mapper.ProjectTo<TResponse>(query)
			.SingleOrDefaultAsync(cancellationToken)
			?? throw new ResourceNotFoundException<TResponse>();
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
