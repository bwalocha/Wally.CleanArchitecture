using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.OData.Query;
using Wally.CleanArchitecture.MicroService.Application.Abstractions;
using Wally.CleanArchitecture.MicroService.WebApi.Extensions;

namespace Wally.CleanArchitecture.MicroService.WebApi.Controllers;

public class QueryOption<TPresentationRequest, TApplicationRequest> : IQueryOptions<TApplicationRequest>
	where TPresentationRequest : Wally.CleanArchitecture.MicroService.WebApi.Abstractions.IRequest
	where TApplicationRequest : IRequest
{
	private readonly ODataQueryOptions<TPresentationRequest> _queryOptions;
	private readonly IMapper _mapper;

	public QueryOption(ODataQueryOptions<TPresentationRequest> queryOptions, IMapper mapper)
	{
		_queryOptions = queryOptions;
		_mapper = mapper;
		Skip = queryOptions.Skip?.Value;
		Top = queryOptions.Top?.Value;
	}

	public int? Skip { get; }
	public int? Top { get; }
	public IQueryable<TEntity> ApplyFilter<TEntity, TResult>(IQueryable<TEntity> query)
	{
		return query.ApplyFilter(_queryOptions, _mapper);
	}

	public IQueryable<TEntity> ApplySearch<TEntity>(IQueryable<TEntity> query, Func<IQueryable<TEntity>, string, IQueryable<TEntity>> search)
	{
		return query.ApplySearch(_queryOptions, search);
	}

	public IQueryable<TEntity> ApplyOrderBy<TEntity>(IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>> applyDefaultOrderBy)
	{
		return query.ApplyOrderBy(_queryOptions, applyDefaultOrderBy, _mapper);
	}

	public IQueryable<TEntity> ApplySkip<TEntity>(IQueryable<TEntity> query)
	{
		return query.ApplySkip(_queryOptions);
	}

	public IQueryable<TEntity> ApplyTop<TEntity>(IQueryable<TEntity> query)
	{
		return query.ApplyTop(_queryOptions);
	}
}
