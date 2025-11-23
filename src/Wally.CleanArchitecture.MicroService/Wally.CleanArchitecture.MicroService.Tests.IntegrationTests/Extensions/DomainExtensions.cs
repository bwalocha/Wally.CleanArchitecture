using System.Linq.Expressions;
using System.Reflection;

namespace Wally.CleanArchitecture.MicroService.Tests.IntegrationTests.Extensions;

public static class DomainExtensions
{
	public static T Set<T, TValue>(this T @object, Expression<Func<T, TValue>> propertyExpression, TValue value)
	{
		if (propertyExpression.Body is not MemberExpression { Member: PropertyInfo propertyInfo, })
		{
			throw new ArgumentException("Expression must be a property access.", nameof(propertyExpression));
		}

		return @object.Set(propertyInfo.Name, value);
	}

	private static T Set<T, TValue>(this T @object, string propertyName, TValue? value)
	{
		var type = typeof(T);
		
		do
		{
			var propertyInfo = type.GetProperty(propertyName);
			if (propertyInfo?.CanWrite == true)
			{
				propertyInfo.SetValue(@object, value);
				return @object;
			}

			type = type.BaseType;
		}
		while (type != null);

		throw new ArgumentException(nameof(@object));
	}
}
