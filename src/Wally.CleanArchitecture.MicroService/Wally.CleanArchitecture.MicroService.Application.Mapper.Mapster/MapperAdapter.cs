/*
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Wally.CleanArchitecture.MicroService.Application.Mapper.Mapster;

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
		return query.ProjectToType<TDestination>(_mapper.Config);
	}

	public IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> query)
		where TSource : class
		where TDestination : class
	{
		return query.ProjectToType<TDestination>(_mapper.Config);
	}

	// public LambdaExpression MapExpression(LambdaExpression expression, Type sourceExpressionType, Type destExpressionType)
	// {
	// 	return _mapper.Config.CreateMapExpression(sourceExpressionType, destExpressionType, expression);
	// }
	//
	// public TDestDelegate MapExpression<TDestDelegate>(LambdaExpression expression)
	// 	where TDestDelegate : LambdaExpression
	// {
	// 	var mapped = _mapper.Config.CreateMapExpression(expression.Type, typeof(TDestDelegate), expression);
	//
	// 	return (TDestDelegate)mapped;
	// }
	
	public LambdaExpression MapExpression(
		LambdaExpression expression,
		Type sourceExpressionType,
		Type destExpressionType)
	{
		// Mapster NIE wspiera bezpośredniego mapowania LambdaExpression jak AutoMapper
		// Jedyne sensowne podejście: użycie projekcji Select z wygenerowanym expression

		var method = typeof(MapperAdapter)
			.GetMethod(nameof(CreateMapExpressionInternal), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!
			.MakeGenericMethod(sourceExpressionType, destExpressionType);

		return (LambdaExpression)method.Invoke(this, new object[] { expression })!;
	}

	public TDestDelegate MapExpression<TDestDelegate>(LambdaExpression expression)
		where TDestDelegate : LambdaExpression
	{
		// brak natywnego wsparcia → fallback
		return (TDestDelegate)MapExpression(
			expression,
			expression.Parameters[0].Type,
			expression.ReturnType);
	}

	// 🔥 kluczowa część
	private Expression<Func<TDestination, bool>> CreateMapExpressionInternal<TSource, TDestination>(
		LambdaExpression sourceExpression)
	{
		// Mapster potrafi wygenerować projection expression:
		var mapExpr = _mapper.Config.GetProjectionExpression<TSource, TDestination>();

		// sourceExpression: Expression<Func<TSource, bool>>
		var sourceLambda = (Expression<Func<TSource, bool>>)sourceExpression;

		// mapExpr: Expression<Func<TSource, TDestination>>
		// chcemy: Expression<Func<TDestination, bool>>

		var param = Expression.Parameter(typeof(TDestination), "dest");

		// odwrotne mapowanie NIE istnieje w Mapster → trzeba zrobić replace
		var body = new ReplaceExpressionVisitor(
			mapExpr.Body,
			param).Visit(sourceLambda.Body);

		return Expression.Lambda<Func<TDestination, bool>>(body!, param);
	}

	// prosty visitor do podmiany expression
	private class ReplaceExpressionVisitor : ExpressionVisitor
	{
		private readonly Expression _source;
		private readonly Expression _target;

		public ReplaceExpressionVisitor(Expression source, Expression target)
		{
			_source = source;
			_target = target;
		}

		public override Expression Visit(Expression node)
		{
			if (node == _source)
				return _target;

			return base.Visit(node);
		}
	}
}
*/
