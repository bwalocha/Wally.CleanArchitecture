using System.Linq;
using System.Linq.Expressions;

namespace Wally.CleanArchitecture.MicroService.Application.Abstractions;

public interface IMapper
{
	IQueryable<TDestination> ProjectTo<TDestination>(IQueryable query)
		where TDestination : class;
	
	IQueryable<TDestination> ProjectTo<TSource, TDestination>(IQueryable<TSource> query)
		where TSource : class
		where TDestination : class;

	public LambdaExpression MapExpression(
		LambdaExpression expression,
		Type sourceExpressionType,
		Type destExpressionType);

	public TDestDelegate MapExpression<TDestDelegate>(LambdaExpression expression)
		where TDestDelegate : LambdaExpression;
}
