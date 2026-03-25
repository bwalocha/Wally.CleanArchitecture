using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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

	public LambdaExpression MapExpression(
		LambdaExpression expression,
		Type sourceExpressionType,
		Type destExpressionType)
	{
		var sourceFuncType = GetExpressionFuncType(sourceExpressionType);
		var destFuncType = GetExpressionFuncType(destExpressionType);

		if (sourceFuncType == null || destFuncType == null)
		{
			throw new NotSupportedException(
				$"MapExpression supports only Expression<Func<TSource, TResult>>. Source: {sourceExpressionType}, Destination: {destExpressionType}");
		}

		var sourceArgs = sourceFuncType.GenericTypeArguments;
		var destArgs = destFuncType.GenericTypeArguments;

		if (sourceArgs.Length != 2 || destArgs.Length != 2)
		{
			throw new NotSupportedException(
				$"MapExpression supports only Func<T, TResult>. Source: {sourceFuncType}, Destination: {destFuncType}");
		}

		if (sourceArgs[1] != destArgs[1] || sourceArgs[1] != typeof(bool))
		{
			var selectorMethod = typeof(MapperAdapter)
				.GetMethod(nameof(MapSelectorInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
				.MakeGenericMethod(sourceArgs[0], destArgs[0]);

			return (LambdaExpression)selectorMethod.Invoke(this, [expression, destArgs[1]])!;
		}

		var method = typeof(MapperAdapter)
			.GetMethod(nameof(MapPredicateInternal), BindingFlags.NonPublic | BindingFlags.Instance)!
			.MakeGenericMethod(sourceArgs[0], destArgs[0]);

		return (LambdaExpression)method.Invoke(this, [expression])!;
	}

	public TDestDelegate MapExpression<TDestDelegate>(LambdaExpression expression)
		where TDestDelegate : LambdaExpression
	{
		var mapped = MapExpression(expression, expression.GetType(), typeof(TDestDelegate));
		return (TDestDelegate)mapped;
	}

	private static Type? GetExpressionFuncType(Type expressionType)
	{
		var current = expressionType;
		while (current != null)
		{
			if (current.IsGenericType && current.GetGenericTypeDefinition() == typeof(Expression<>))
			{
				var candidate = current.GenericTypeArguments[0];
				if (candidate.IsGenericType && candidate.GetGenericTypeDefinition() == typeof(Func<,>))
				{
					return candidate;
				}

				return null;
			}

			current = current.BaseType;
		}

		return null;
	}

	private Expression<Func<TDestination, bool>> MapPredicateInternal<TSource, TDestination>(
		LambdaExpression expression)
	{
		if (expression is not Expression<Func<TSource, bool>> sourcePredicate)
		{
			throw new NotSupportedException(
				$"Expected Expression<Func<{typeof(TSource).Name}, bool>>, got: {expression.Type}");
		}

		return MapLambda<TSource, TDestination, bool>(sourcePredicate);
	}

	private Expression<Func<TDestination, TResult>> MapLambda<TSource, TDestination, TResult>(
		Expression<Func<TSource, TResult>> sourceLambda)
	{
		var sourceParam = sourceLambda.Parameters[0];
		var destinationParam = Expression.Parameter(typeof(TDestination), sourceParam.Name ?? "x");
		var rewrittenBody = new SourceToDestinationMemberVisitor<TSource, TDestination>(sourceParam, destinationParam)
			.Visit(sourceLambda.Body)!;

		return Expression.Lambda<Func<TDestination, TResult>>(rewrittenBody, destinationParam);
	}

	private LambdaExpression MapSelectorInternal<TSource, TDestination>(
		LambdaExpression expression,
		Type destinationResultType)
	{
		var sourceFuncType = GetExpressionFuncType(expression.GetType())!;
		var sourceResultType = sourceFuncType.GenericTypeArguments[1];
		var method = typeof(MapperAdapter)
			.GetMethod(nameof(MapSelectorInternalGeneric), BindingFlags.NonPublic | BindingFlags.Instance)!
			.MakeGenericMethod(typeof(TSource), typeof(TDestination), sourceResultType, destinationResultType);

		return (LambdaExpression)method.Invoke(this, [expression])!;
	}

	private Expression<Func<TDestination, TDestinationResult>> MapSelectorInternalGeneric<TSource, TDestination, TSourceResult, TDestinationResult>(
		LambdaExpression expression)
	{
		if (expression is not Expression<Func<TSource, TSourceResult>> sourceSelector)
		{
			throw new NotSupportedException(
				$"Expected Expression<Func<{typeof(TSource).Name}, {typeof(TSourceResult).Name}>>, got: {expression.Type}");
		}

		var mapped = MapLambda<TSource, TDestination, TSourceResult>(sourceSelector);
		var body = mapped.Body;

		if (body.Type != typeof(TDestinationResult))
		{
			body = ConvertExpression(body, typeof(TDestinationResult));
		}

		return Expression.Lambda<Func<TDestination, TDestinationResult>>(body, mapped.Parameters);
	}

	private static Expression ConvertExpression(Expression expression, Type destinationType)
	{
		if (expression.Type == destinationType)
		{
			return expression;
		}

		var valueProperty = expression.Type.GetProperty("Value");
		if (valueProperty != null && valueProperty.PropertyType == destinationType)
		{
			return Expression.Property(expression, valueProperty);
		}

		var constructor = destinationType.GetConstructor([expression.Type]);
		if (constructor != null)
		{
			return Expression.New(constructor, expression);
		}

		if (destinationType.IsAssignableFrom(expression.Type))
		{
			return expression;
		}

		return Expression.Convert(expression, destinationType);
	}

	private sealed class SourceToDestinationMemberVisitor<TSource, TDestination> : ExpressionVisitor
	{
		private readonly ParameterExpression _source;
		private readonly ParameterExpression _destination;

		public SourceToDestinationMemberVisitor(ParameterExpression source, ParameterExpression destination)
		{
			_source = source;
			_destination = destination;
		}

		protected override Expression VisitMember(MemberExpression node)
		{
			if (node.Expression == _source)
			{
				var destinationProperty = typeof(TDestination).GetProperty(node.Member.Name);
				if (destinationProperty != null)
				{
					return Expression.Property(_destination, destinationProperty);
				}

				var destinationField = typeof(TDestination).GetField(node.Member.Name);
				if (destinationField != null)
				{
					return Expression.Field(_destination, destinationField);
				}

				throw new NotSupportedException(
					$"Cannot map member '{node.Member.Name}' from {typeof(TSource).Name} to {typeof(TDestination).Name}.");
			}

			return base.VisitMember(node);
		}

		protected override Expression VisitBinary(BinaryExpression node)
		{
			var left = Visit(node.Left);
			var right = Visit(node.Right);

			if (left.Type != right.Type)
			{
				right = TryConvertForBinary(right, left.Type) ?? right;
				if (left.Type != right.Type)
				{
					left = TryConvertForBinary(left, right.Type) ?? left;
				}
			}

			if (node.NodeType is ExpressionType.Equal or ExpressionType.NotEqual)
			{
				return Expression.MakeBinary(node.NodeType, left, right, node.IsLiftedToNull, null);
			}

			return node.Update(left, node.Conversion, right);
		}

		private static Expression? TryConvertForBinary(Expression expression, Type destinationType)
		{
			if (expression.Type == destinationType)
			{
				return expression;
			}

			var destinationUnderlying = Nullable.GetUnderlyingType(destinationType);
			if (destinationUnderlying != null)
			{
				if (expression.Type == destinationUnderlying)
				{
					return Expression.Convert(expression, destinationType);
				}

				var sourceUnderlying = Nullable.GetUnderlyingType(expression.Type);
				if (sourceUnderlying != null)
				{
					var hasValue = Expression.Property(expression, "HasValue");
					var sourceValue = Expression.Property(expression, "Value");
					var convertedUnderlying = TryConvertForBinary(sourceValue, destinationUnderlying);
					if (convertedUnderlying != null)
					{
						return Expression.Condition(
							hasValue,
							Expression.Convert(convertedUnderlying, destinationType),
							Expression.Constant(null, destinationType));
					}
				}
			}

			var sourceUnderlyingOnly = Nullable.GetUnderlyingType(expression.Type);
			if (sourceUnderlyingOnly != null && sourceUnderlyingOnly == destinationType)
			{
				return Expression.Property(expression, "Value");
			}

			var valueProperty = expression.Type.GetProperty("Value");
			if (valueProperty != null)
			{
				if (valueProperty.PropertyType == destinationType)
				{
					return Expression.Property(expression, valueProperty);
				}

				if (destinationUnderlying != null && valueProperty.PropertyType == destinationUnderlying)
				{
					return Expression.Convert(Expression.Property(expression, valueProperty), destinationType);
				}
			}

			var constructor = destinationType.GetConstructor([expression.Type,]);
			if (constructor != null)
			{
				return Expression.New(constructor, expression);
			}

			if (sourceUnderlyingOnly != null)
			{
				var ctorFromUnderlying = destinationType.GetConstructor([sourceUnderlyingOnly,]);
				if (ctorFromUnderlying != null)
				{
					return Expression.New(ctorFromUnderlying, Expression.Property(expression, "Value"));
				}
			}

			if (destinationType.IsAssignableFrom(expression.Type))
			{
				return expression;
			}

			return null;
		}
	}
}
