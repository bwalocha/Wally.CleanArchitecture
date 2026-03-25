using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper.Extensions.ExpressionMapping;
using AutoMapper.QueryableExtensions;

namespace Wally.CleanArchitecture.MicroService.Application.Mapper.AutoMapper;

public class MapperAdapter : Wally.CleanArchitecture.MicroService.Application.Abstractions.IMapper
{
	private readonly IMapper _mapper;

	public MapperAdapter(IMapper mapper)
	{
		_mapper = mapper;
	}

	public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable query)
		where TDestination : class
	{
		return query.ProjectTo<TDestination>(_mapper.ConfigurationProvider);
	}

	public IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> query)
		where TSource : class
		where TDestination : class
	{
		return query.ProjectTo<TDestination>(_mapper.ConfigurationProvider);
	}

	public LambdaExpression MapExpression(LambdaExpression expression, Type sourceExpressionType, Type destExpressionType)
	{
		return _mapper.MapExpression(expression, sourceExpressionType, destExpressionType);
	}

	public TDestDelegate MapExpression<TDestDelegate>(LambdaExpression expression)
		where TDestDelegate : LambdaExpression
	{
		return _mapper.MapExpression<TDestDelegate>(expression);
	}
}
